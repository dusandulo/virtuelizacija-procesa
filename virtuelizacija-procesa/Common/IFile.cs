using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IFile
    {
        [OperationContract]
        void ParseFile(FileHandle options, bool isForecast);
        [OperationContract]
        void Calc();
    }
}
