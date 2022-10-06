using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Informer.Areas.hf.Models
{
    public static class AnonymousTypeConversion 
    {
        /// <summary>
        /// Преобразует один объект DbDataRwcord во что-то другое.
        /// Конечный тип должен иметь конструктор по умолчанию.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="record"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(this DbDataRecord record)
        {
            T item = Activator.CreateInstance<T>();
            for (int f = 0; f < record.FieldCount; f++)
            {
                var rr = record.GetName(f);
                PropertyInfo p = item.GetType().GetProperty(rr);
                if (p != null && p.PropertyType == record.GetFieldType(f))
                {
                    p.SetValue(item, record.GetValue(f), null);
                }
            }

            return item;
        }

        /// <summary>
        /// Преобразует список DbDataRecord к списку что-то еще.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> ConvertTo<T>(this List<DbDataRecord> list)
        {
            List<T> result = (List<T>)Activator.CreateInstance<List<T>>();

            list.ForEach(rec =>
            {
                result.Add(rec.ConvertTo<T>());
            });

            return result;
        }
    }
}