using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reversi.Persistance
{
    public class ReversiFileDataAccess : IReversiDataAccess
    {
        public async Task<ReversiTable> LoadAsync(String path)
        {
            string savedGame = await File.ReadAllTextAsync(path);
            return new ReversiTable(JsonSerializer.Deserialize<SaveReversiTable>(savedGame)!);
        }

        public async Task SaveAsync(String path, ReversiTable table)
        {
            string saveString = JsonSerializer.Serialize(new SaveReversiTable(table));
            await File.WriteAllTextAsync(path, saveString); 
        }
    }
}
