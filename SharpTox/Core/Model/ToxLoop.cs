using System;
using System.Threading;
using SharpTox.Core.Interfaces;

namespace SharpTox.Core
{
    public class ToxLoop : IDisposable
    {
        private bool disposed;
        private ITox tox;

        private ToxLoop(ITox tox)
        {
            this.tox = tox;
        }

        public static ToxLoop Start([NotNull] ITox tox)
        {
            var toxLoop = new ToxLoop(tox);
            toxLoop.Loop();
            return toxLoop;
        }

        private void Loop()
        {
            var thread = new Thread(() =>
            {
                while (!this.disposed)
                {
                    var time = this.tox.Iterate();
                    Thread.Sleep(time);
                }
            });

            thread.Start();
        }

        void IDisposable.Dispose()
        {
            if (!this.disposed)
            {
                this.tox = null;
                this.disposed = true;
            }
        }
    }
}
