using System.Collections.Generic;
using Prediction.Core.Models;

namespace Prediction.Core.Services
{
    public interface IDataInputService
    {
        ICollection<DataInput> ReadFile(string path);
    }
}