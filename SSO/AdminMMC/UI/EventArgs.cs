using System;

namespace GT.BizTalk.SSO.AdminMMC
{
    public class EventArgs<V> : EventArgs
    {
        private V value;

        public V Value
        {
            get
            {
                return this.value;
            }
        }

        public EventArgs(V value)
        {
            this.value = value;
        }
    }

    public class EventArgs<V, R> : EventArgs
    {
        private V value;
        private R result;

        public V Value
        {
            get
            {
                return this.value;
            }
        }

        public R Result
        {
            get
            {
                return this.result;
            }
            set
            {
                this.result = value;
            }
        }

        public EventArgs(V value)
        {
            this.value = value;
        }
    }

    public class ResultEventArgs<R> : EventArgs
    {
        private R result;

        public R Result
        {
            get
            {
                return this.result;
            }
            set
            {
                this.result = value;
            }
        }

        public ResultEventArgs()
        {
        }
    }
}
