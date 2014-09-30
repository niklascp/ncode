using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.JobScheduling
{
    public interface IBackgroundJob
    {
        void Execute();
    }

    public interface IBackgroundJob<P> where P : IBackgroundJobParameters
    {
        void Execute(P parameters);
    }

    public interface IBackgroundJobParameters { }
}
