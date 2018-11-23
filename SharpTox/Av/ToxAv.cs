using SharpTox.Core;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SharpTox.Av
{
    /// <summary>
    /// Represents an instance of toxav.
    /// </summary>
    public sealed class ToxAv : IDisposable
    {
        private List<ToxAvDelegates.GroupAudioReceiveDelegate> groupAudioHandlers = new List<ToxAvDelegates.GroupAudioReceiveDelegate>();
        private bool disposed = false;
        private bool running = false;
        private CancellationTokenSource cancelTokenSource;
        private Action release;

        /// <summary>
        /// The Tox instance that this toxav instance belongs to.
        /// </summary>
        private ToxHandle toxHandle;

        /// <summary>
        /// The handle of this toxav instance.
        /// </summary>
        internal ToxAvHandle Handle { get; }

        /// <summary>
        /// Initialises a new instance of toxav.
        /// </summary>
        /// <param name="tox"></param>
        public ToxAv(Tox tox)
        {
            if (tox == null)
            {
                throw new ArgumentNullException(nameof(tox));
            }

            if (tox.Handle.IsInvalid)
            {
                throw new ArgumentException(nameof(tox));
            }

            this.toxHandle = tox.Handle;

            var error = ToxAvErrorNew.Ok;
            this.Handle = ToxAvFunctions.New(tox.Handle, ref error);

            if (this.Handle == null || this.Handle.IsInvalid || error != ToxAvErrorNew.Ok)
            {
                throw new Exception("Could not create a new instance of toxav.");
            }

            //register audio/video callbacks early on
            //due to toxav being silly, we can't start calls without registering those beforehand
            this.release = RegisterAudioVideoCallbacks();

            Action RegisterAudioVideoCallbacks()
            {
                this.OnVideoFrameReceived += MockVideoFrameReceive;
                this.OnAudioFrameReceived += MockAudioFrameReceive;

                return () =>
                {
                    this.OnVideoFrameReceived -= MockVideoFrameReceive;
                    this.OnAudioFrameReceived -= MockAudioFrameReceive;
                };

                void MockVideoFrameReceive(object sender, ToxAvEventArgs.VideoFrameEventArgs e) { }
                void MockAudioFrameReceive(object sender, ToxAvEventArgs.AudioFrameEventArgs e) { }
            }
        }

        /// <summary>
        /// Releases all resources used by this instance of tox.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //dispose pattern as described on msdn for a class that uses a safe handle
        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                this.release?.Invoke();
                this.release = null;

                if (cancelTokenSource != null)
                {
                    try
                    {
                        cancelTokenSource.Cancel();
                        cancelTokenSource.Dispose();
                    }
                    catch (ObjectDisposedException) { }

                    this.cancelTokenSource = null;
                }
            }

            if (Handle != null && !Handle.IsInvalid && !Handle.IsClosed)
            {
                this.Handle.Dispose();
            }

            disposed = true;
        }

        /// <summary>
        /// Starts the main toxav_do loop.
        /// </summary>
        public void Start()
        {
            ThrowIfDisposed();

            if (running)
            {
                return;
            }

            Loop();
        }

        /// <summary>
        /// Stops the main toxav_do loop if it's running.
        /// </summary>
        public void Stop()
        {
            ThrowIfDisposed();

            if (!running)
            {
                return;
            }

            if (cancelTokenSource != null)
            {
                cancelTokenSource.Cancel();
                cancelTokenSource.Dispose();
                cancelTokenSource = null;

                running = false;
            }
        }

        private void Loop()
        {
            cancelTokenSource = new CancellationTokenSource();
            running = true;

            Task.Factory.StartNew(async () =>
            {
                while (running)
                {
                    if (cancelTokenSource.IsCancellationRequested)
                    {
                        break;
                    }

                    var delay = DoIterate();
                    await Task.Delay((int)(delay.TotalMilliseconds / 4));
                }
            }, cancelTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// Runs the loop once in the current thread and returns the next timeout.
        /// </summary>
        public TimeSpan Iterate()
        {
            ThrowIfDisposed();

            if (running)
            {
                throw new Exception("Loop already running");
            }

            return this.DoIterate();
        }

        private TimeSpan DoIterate()
        {
            ToxAvFunctions.Iterate(Handle);
            return TimeSpan.FromMilliseconds(ToxAvFunctions.IterationInterval(Handle));
        }

        public bool Call(uint friendNumber, uint audioBitrate, uint videoBitrate, out ToxAvErrorCall error)
        {
            ThrowIfDisposed();

            error = ToxAvErrorCall.Ok;
            return ToxAvFunctions.Call(Handle, friendNumber, audioBitrate, videoBitrate, ref error);
        }

        public bool Answer(uint friendNumber, uint audioBitrate, uint videoBitrate, out ToxAvErrorAnswer error)
        {
            ThrowIfDisposed();

            error = ToxAvErrorAnswer.Ok;
            return ToxAvFunctions.Answer(Handle, friendNumber, audioBitrate, videoBitrate, ref error);
        }

        public bool SendControl(uint friendNumber, ToxAvCallControl control, out ToxAvErrorCallControl error)
        {
            ThrowIfDisposed();

            error = ToxAvErrorCallControl.Ok;
            return ToxAvFunctions.CallControl(Handle, friendNumber, control, ref error);
        }

        public bool SetAudioBitrate(uint friendNumber, uint bitrate, out ToxAvErrorBitRateSet error)
        {
            ThrowIfDisposed();

            error = ToxAvErrorBitRateSet.Ok;
            return ToxAvFunctions.Audio.SetBitrate(Handle, friendNumber, bitrate, ref error);
        }

        public bool SetVideoBitrate(uint friendNumber, uint bitrate, out ToxAvErrorBitRateSet error)
        {
            ThrowIfDisposed();

            error = ToxAvErrorBitRateSet.Ok;
            return ToxAvFunctions.Video.SetBitrate(Handle, friendNumber, bitrate, ref error);
        }

        public bool SendVideoFrame(uint friendNumber, ToxAvVideoFrame frame, out ToxAvErrorSendFrame error)
        {
            ThrowIfDisposed();
            error = ToxAvErrorSendFrame.Ok;
            return ToxAvFunctions.Video.SendFrame(Handle, friendNumber, frame.Width, frame.Height, frame.Y, frame.U, frame.V, ref error);
        }

        public bool SendAudioFrame(uint friendNumber, ToxAvAudioFrame frame, out ToxAvErrorSendFrame error)
        {
            ThrowIfDisposed();

            error = ToxAvErrorSendFrame.Ok;
            return ToxAvFunctions.Audio.SendFrame(Handle, friendNumber, frame.Data, (uint)(frame.Data.Length / frame.Channels), (byte)frame.Channels, (uint)frame.SamplingRate, ref error);
        }

        /// <summary>
        /// Creates a new audio groupchat.
        /// </summary>
        /// <returns></returns>
        public int AddAvGroupchat()
        {
            ThrowIfDisposed();

            ToxAvDelegates.GroupAudioReceiveDelegate callback = (tox, groupNumber, peerNumber, frame, sampleCount, channels, sampleRate, userData) =>
            {
                if (OnReceivedGroupAudio != null)
                {
                    short[] samples = new short[sampleCount * channels];
                    Marshal.Copy(frame, samples, 0, samples.Length);

                    OnReceivedGroupAudio(this, new ToxAvEventArgs.GroupAudioDataEventArgs(groupNumber, peerNumber, samples, channels, sampleRate));
                }
            };

            int result = ToxAvFunctions.AddAvGroupchat(toxHandle, callback, IntPtr.Zero);
            if (result != -1)
            {
                groupAudioHandlers.Add(callback);
            }

            return result;
        }

        /// <summary>
        /// Joins an audio groupchat.
        /// </summary>
        /// <param name="friendNumber"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int JoinAvGroupchat(uint friendNumber, byte[] data)
        {
            ThrowIfDisposed();

            if (data == null)
                throw new ArgumentNullException("data");

            ToxAvDelegates.GroupAudioReceiveDelegate callback = (IntPtr tox, uint groupNumber, uint peerNumber, IntPtr frame, uint sampleCount, byte channels, uint sampleRate, IntPtr userData) =>
            {
                if (OnReceivedGroupAudio != null)
                {
                    short[] samples = new short[sampleCount * channels];
                    Marshal.Copy(frame, samples, 0, samples.Length);

                    OnReceivedGroupAudio(this, new ToxAvEventArgs.GroupAudioDataEventArgs(groupNumber, peerNumber, samples, channels, sampleRate));
                }
            };

            int result = ToxAvFunctions.JoinAvGroupchat(toxHandle, friendNumber, data, (ushort)data.Length, callback, IntPtr.Zero);
            if (result != -1)
            {
                groupAudioHandlers.Add(callback);
            }

            return result;
        }

        /// <summary>
        /// Sends an audio frame to a group.
        /// </summary>
        /// <param name="groupNumber"></param>
        /// <param name="pcm"></param>
        /// <param name="perframe"></param>
        /// <param name="channels"></param>
        /// <param name="sampleRate"></param>
        /// <returns></returns>
        public bool GroupSendAudio(uint groupNumber, short[] pcm, uint perframe, byte channels, uint sampleRate)
        {
            ThrowIfDisposed();

            return ToxAvFunctions.GroupSendAudio(toxHandle, groupNumber, pcm, perframe, channels, sampleRate) == 0;
        }

        #region Events
        private readonly ToxAvCallbackHandler<ToxAvEventArgs.CallRequestEventArgs, ToxAvDelegates.CallDelegate> callRequestReceive
            = new ToxAvCallbackHandler<ToxAvEventArgs.CallRequestEventArgs, ToxAvDelegates.CallDelegate>(ToxAvCallbacks.Call, cb =>
                     (toxAv, friendNumber, audioEnabled, videoEnabled, userData) =>
                            cb(new ToxAvEventArgs.CallRequestEventArgs(friendNumber, audioEnabled, videoEnabled)));

        /// <summary>
        /// Occurs when a friend sends a call request.
        /// </summary>
        public event EventHandler<ToxAvEventArgs.CallRequestEventArgs> OnCallRequestReceived {
            add => this.callRequestReceive.Add(this, value);
            remove => this.callRequestReceive.Remove(this, value);
        }

        private readonly ToxAvCallbackHandler<ToxAvEventArgs.CallStateEventArgs, ToxAvDelegates.CallStateDelegate> callStateChange
            = new ToxAvCallbackHandler<ToxAvEventArgs.CallStateEventArgs, ToxAvDelegates.CallStateDelegate>(ToxAvCallbacks.CallState, cb =>
                     (toxAv, friendNumber, state, userData) => cb(new ToxAvEventArgs.CallStateEventArgs(friendNumber, state)));

        /// <summary>
        /// Occurs when the state of a call changed.
        /// </summary>
        public event EventHandler<ToxAvEventArgs.CallStateEventArgs> OnCallStateChanged {
            add => this.callStateChange.Add(this, value);
            remove => this.callStateChange.Remove(this, value);
        }

        //private EventHandler<ToxAvEventArgs.BitrateStatusEventArgs> _onBitrateSuggestion;

        ///// <summary>
        ///// Occurs when a friend changed their bitrate during a call.
        ///// </summary>
        //public event EventHandler<ToxAvEventArgs.BitrateStatusEventArgs> OnBitrateSuggestion
        //{
        //    add
        //    {
        //        if (_onBitrateStatusCallback == null)
        //        {
        //            _onBitrateStatusCallback = (IntPtr toxAv, uint friendNumber, uint audioBitrate, uint videoBitrate, IntPtr userData) =>
        //            {
        //                    if (_onBitrateSuggestion != null)
        //                        _onBitrateSuggestion(this, new ToxAvEventArgs.BitrateStatusEventArgs(ToxTools.Map(friendNumber), (int)audioBitrate, (int)videoBitrate));
        //            };

        //            ToxAvFunctions.RegisterBitrateStatusCallback(Handle, _onBitrateStatusCallback, IntPtr.Zero);
        //        }

        //        _onBitrateSuggestion += value;
        //    }
        //    remove
        //    {
        //        if (_onBitrateSuggestion.GetInvocationList().Length == 1)
        //        {
        //            ToxAvFunctions.RegisterBitrateStatusCallback(Handle, null, IntPtr.Zero);
        //            _onBitrateStatusCallback = null;
        //        }

        //        _onBitrateSuggestion -= value;
        //    }
        //}

        private readonly ToxAvCallbackHandler<ToxAvEventArgs.VideoBitrateEventArgs, ToxAvDelegates.VideoBitRateDelegate> videoBitrateChange
            = new ToxAvCallbackHandler<ToxAvEventArgs.VideoBitrateEventArgs, ToxAvDelegates.VideoBitRateDelegate>(ToxAvCallbacks.Video.BitRate, cb =>
                (toxAv, friendNumber, bitrate, userData) =>
                        cb(new ToxAvEventArgs.VideoBitrateEventArgs(friendNumber, bitrate)));

        public event EventHandler<ToxAvEventArgs.VideoBitrateEventArgs> OnVideoBitrateChanged {
            add => this.videoBitrateChange.Add(this, value);
            remove => this.videoBitrateChange.Remove(this, value);
        }

        private readonly ToxAvCallbackHandler<ToxAvEventArgs.AudioBitrateEventArgs, ToxAvDelegates.AudioBitRateDelegate> audioBitrateChange
            = new ToxAvCallbackHandler<ToxAvEventArgs.AudioBitrateEventArgs, ToxAvDelegates.AudioBitRateDelegate>(ToxAvCallbacks.Audio.BitRate, cb =>
                (toxAv, friendNumber, bitrate, userData) =>
                        cb(new ToxAvEventArgs.AudioBitrateEventArgs(friendNumber, bitrate)));

        public event EventHandler<ToxAvEventArgs.AudioBitrateEventArgs> OnAudioBitrateChanged {
            add => this.audioBitrateChange.Add(this, value);
            remove => this.audioBitrateChange.Remove(this, value);
        }

        private readonly ToxAvCallbackHandler<ToxAvEventArgs.VideoFrameEventArgs, ToxAvDelegates.VideoReceiveFrameDelegate> videoFrameReceive
            = new ToxAvCallbackHandler<ToxAvEventArgs.VideoFrameEventArgs, ToxAvDelegates.VideoReceiveFrameDelegate>(ToxAvCallbacks.Video.ReceiveFrame, cb =>
                (toxAv, friendNumber, width, height, y, u, v, yStride, uStride, vStride, userData) =>
                        cb(new ToxAvEventArgs.VideoFrameEventArgs(friendNumber, new ToxAvVideoFrame(width, height, y, u, v, yStride, uStride, vStride))));

        /// <summary>
        /// Occurs when an video frame is received.
        /// </summary>
        public event EventHandler<ToxAvEventArgs.VideoFrameEventArgs> OnVideoFrameReceived {
            add => this.videoFrameReceive.Add(this, value);
            remove => this.videoFrameReceive.Remove(this, value);
        }

        private readonly ToxAvCallbackHandler<ToxAvEventArgs.AudioFrameEventArgs, ToxAvDelegates.AudioReceiveFrameDelegate> audioFrameReceive
            = new ToxAvCallbackHandler<ToxAvEventArgs.AudioFrameEventArgs, ToxAvDelegates.AudioReceiveFrameDelegate>(ToxAvCallbacks.Audio.ReceiveFrame, cb =>
                (toxAv, friendNumber, pcm, sampleCount, channels, samplingRate, userData) =>
                        cb(new ToxAvEventArgs.AudioFrameEventArgs(friendNumber, new ToxAvAudioFrame(pcm, sampleCount, samplingRate, channels))));

        /// <summary>
        /// Occurs when an audio frame is received.
        /// </summary>
        public event EventHandler<ToxAvEventArgs.AudioFrameEventArgs> OnAudioFrameReceived {
            add => this.audioFrameReceive.Add(this, value);
            remove => this.audioFrameReceive.Remove(this, value);
        }

        /// <summary>
        /// Occurs when an audio frame was received from a group.
        /// </summary>
        public event EventHandler<ToxAvEventArgs.GroupAudioDataEventArgs> OnReceivedGroupAudio;

        #endregion

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}
