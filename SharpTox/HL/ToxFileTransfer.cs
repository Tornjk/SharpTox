﻿using System;
using System.IO;
using SharpTox.Core;

namespace SharpTox.HL
{
    public class ToxFileTransfer
    {
        public ToxHL Tox { get; private set; }
        public ToxFileInfo Info { get; private set; }
        public ToxFriend Friend { get; private set; }
        public string Name { get; private set; }
        public long Size { get; private set; }
        public ToxFileKind Kind { get; private set; }

        public bool IsPaused { get; protected set; } = true; //initial state is 'paused' for the receiving end, 'running' for the sending end
        public bool IsFinished { get; private set; }
        public bool IsCanceled { get; private set; }

        public event EventHandler<EventArgs> OnPaused;
        public event EventHandler<EventArgs> OnResumed;
        public event EventHandler<EventArgs> OnCanceled;
        public event EventHandler<EventArgs> OnFinished;

        protected readonly Stream _stream;

        internal ToxFileTransfer(ToxHL tox, Stream stream, ToxFriend friend, ToxFileInfo info, string name, ToxFileKind kind)
        {
            Tox = tox;
            Friend = friend;
            Info = info;
            Name = name;
            Size = stream.Length;
            Kind = kind;

            _stream = stream;

            Tox.Core.OnFileControlReceived += OnFileControlReceived;
        }

        internal ToxFileTransfer(ToxHL tox, ToxFriend friend, ToxFileInfo info, string name, long size, ToxFileKind kind)
        {
            Tox = tox;
            Friend = friend;
            Info = info;
            Name = name;
            Size = size;
            Kind = kind;

            Tox.Core.OnFileControlReceived += OnFileControlReceived;
        }

        private void OnFileControlReceived (object sender, ToxEventArgs.FileControlEventArgs e)
        {
            if (e.FileNumber != Info.Number || e.FriendNumber != Friend.Number)
                return;

            switch (e.Control)
            {
                case ToxFileControl.Pause:
                    OnPaused(this, new EventArgs());
                    break;
                case ToxFileControl.Resume:
                    OnResumed(this, new EventArgs());
                    break;
                case ToxFileControl.Cancel:
                    OnCanceled(this, new EventArgs());
                    break;
            }
        }

        public void Pause()
        {
            SendControl(ToxFileControl.Pause);
            OnFileControlReceived(null, new ToxEventArgs.FileControlEventArgs(Friend.Number, Info.Number, ToxFileControl.Pause));
        }

        public void Resume()
        {
            SendControl(ToxFileControl.Resume);
            OnFileControlReceived(null, new ToxEventArgs.FileControlEventArgs(Friend.Number, Info.Number, ToxFileControl.Resume));
        }

        public void Cancel()
        {
            SendControl(ToxFileControl.Cancel);
            OnFileControlReceived(null, new ToxEventArgs.FileControlEventArgs(Friend.Number, Info.Number, ToxFileControl.Cancel));
        }

        protected void SendControl(ToxFileControl control)
        {
            var error = ToxErrorFileControl.Ok;
            Tox.Core.FileControl(Friend.Number, Info.Number, control, out error);

            if (error != ToxErrorFileControl.Ok)
                throw new ToxException<ToxErrorFileControl>(error);
        }
    }
}