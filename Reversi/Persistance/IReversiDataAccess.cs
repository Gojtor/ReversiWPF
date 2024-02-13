using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Persistance
{
    public interface IReversiDataAccess
    {
        Task<ReversiTable> LoadAsync(String path);

        Task SaveAsync(String path, ReversiTable table);
    }
}
