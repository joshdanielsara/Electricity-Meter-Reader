using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace App7
{
    public class SQLiteHelper
    {
        SQLiteAsyncConnection db;
        public SQLiteHelper(string dbPath)
        {
            db = new SQLiteAsyncConnection(dbPath);
            db.CreateTableAsync<Per>().Wait();
        }

        //Insert and Update new record  
        public Task<int> SaveItemAsync(Per person)
        {
            if (person.MeterNo != 0)
            {
                return db.UpdateAsync(person);
            }
            else
            {
                return db.InsertAsync(person);
            }
        }

        //Delete  
        public Task<int> DeleteItemAsync(Per person)
        {
            return db.DeleteAsync(person);
        }

        //Read All Items  
        public Task<List<Per>> GetItemsAsync()
        {
            return db.Table<Per>().ToListAsync();
        }


        //Read Item  
        public Task<Per> GetItemAsync(int personId)
        {
            return db.Table<Per>().Where(i => i.MeterNo == personId).FirstOrDefaultAsync();
        }
    }

}
