using System.Collections.Generic;
using Prediction.Core.Models;

namespace Prediction.Core.Services
{
    public interface IGameMatchService
    {
        void Prepare();
        ICollection<DataInput> GetAll();
        ICollection<GameMatch> Bulk(ICollection<DataInput> matches);
    }
}