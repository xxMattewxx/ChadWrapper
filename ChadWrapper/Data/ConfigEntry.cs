using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace ChadWrapper.Data
{
    class ConfigEntry
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }

        public bool Save()
        {
            using var dbContext = new ConfigDBContext();

            if (Get(Key) == null)
                dbContext.Config.Add(this);
            else
                dbContext.Config.Update(this);

            return dbContext.SaveChanges() > 0;
        }

        public static ConfigEntry Get(string key)
        {
            using var dbContext = new ConfigDBContext();

            return dbContext.Config.Find(key);
        }
    }
}
