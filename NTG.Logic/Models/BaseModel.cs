using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace NTG.Logic.Models
{
    public abstract class BaseModel<T> where T : class
    {
        protected abstract bool IsNew();

        public static DbSet<T> Query
        {
            get{
                var conn = new NTGDB();
                conn.Configuration.ProxyCreationEnabled = false;
                return conn.Set<T>();
            }
        }

        public void Save(NTGDBTransactional transactionConn = null)
        {
            if (transactionConn != null)
            {
                InsertOrUpdate(transactionConn);
            }
            else {
                using (var conn = new NTGDB())
                {
                    InsertOrUpdate(conn);
                }
            }
        }

        private void InsertOrUpdate(NTGDB conn) {
            conn.Configuration.AutoDetectChangesEnabled = false;
            var save = false;

            if (IsNew())
            {
                conn.Set<T>().Add(this as T);
                save = true;
            }
            else
            {
                var entity = this as T;
                var entry = conn.Entry(entity);
                var navProperties = new List<PropertyInfo>();

                foreach (var navProperty in GetType().GetProperties().Where(t => t.PropertyType != null && t.PropertyType.BaseType != null && t.PropertyType.BaseType.IsConstructedGenericType))
                {
                    if (navProperty != null && navProperty.PropertyType.BaseType.GetGenericTypeDefinition() == typeof(BaseModel<>))
                    {
                        navProperty.SetValue(this, null);
                        navProperties.Add(navProperty);
                    }
                }

                if (entry.State == EntityState.Detached)
                {
                    conn.Set<T>().Attach(entity);
                    entry = conn.Entry<T>(entity);
                }

                var dbEntry = entry.GetDatabaseValues();
                foreach (var property in entry.OriginalValues.PropertyNames)
                {
                    var original = dbEntry.GetValue<object>(property);
                    var current = entry.CurrentValues.GetValue<object>(property);
                    if (!Equals(original, current))
                    {
                        entry.Property(property).IsModified = true;
                        save = true;
                    }
                }

                foreach (var navProperty in navProperties)
                {
                    entry.Reference(navProperty.Name).Load();
                }
            }

            if (save)
            {
                conn.SaveChanges();
            }
        }

        public void Delete(NTGDBTransactional transactionConn = null)
        {
            if (transactionConn != null)
            {
                DeleteOrUpdate(transactionConn);
            }
            else
            {
                using (var conn = new NTGDB())
                {
                    DeleteOrUpdate(conn);
                }
            }
        }

        private void DeleteOrUpdate(NTGDB conn)
        {
            var entity = this as T;
            var entry = conn.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                conn.Set<T>().Attach(entity);
                entry = conn.Entry(entity);
            }

            PropertyInfo propInfo = GetType().GetProperty("Active");
            if (propInfo != null)
            {
                propInfo.SetValue(this, false);
                entry.State = EntityState.Modified;
            }
            else
            {
                conn.Set<T>().Remove(this as T);
            }

            conn.SaveChanges();
        }
    }
}
