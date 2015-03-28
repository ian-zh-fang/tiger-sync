using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COM.TIGER.SYNC.TaskJob
{
    public abstract class Job<T> : TaskJob.IJob where T : new()
    {
        public virtual void Executed(DAO.DataHandler dbFrom, DAO.DataHandler dbTarget, string dataFromCmdString)
        {
            Console.WriteLine("## start time {0}----------------------------------------------", DateTime.Now);

            try
            {
                using (System.Data.IDataReader reader = dbFrom.ExecuteReader(dataFromCmdString))
                {
                    while (reader.Read())
                    {
                        var t = Read(reader);
                        Executed(t, dbTarget);
                    }
                }
            }
            catch (Exception e) { Console.WriteLine("错误：{0}", e.Message); }

            Console.WriteLine("## end time {0}----------------------------------------------", DateTime.Now);
        }

        protected virtual T Read(System.Data.IDataReader reader)
        {
            var tp = typeof(T);
            var properties = tp.GetProperties();

            var t = new T();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var property = properties.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
                if (!reader.IsDBNull(i) && property != null && property.CanWrite)
                {
                    var obj = reader.GetValue(i);
                    property.SetValue(t, obj, null);
                }
            }

            return t;
        }

        protected virtual T GetEntity(DAO.DataHandler db, string cmdStr)
        {
            try
            {
                var t = default(T);
                using (System.Data.IDataReader reader = db.ExecuteReader(cmdStr))
                {
                    while (reader.Read())
                    {
                        t = Read(reader);
                        break;
                    }
                }
                return t;
            }
            catch (Exception e) 
            { 
                Console.WriteLine("错误：{0}", e.Message);

                return default(T);
            }
        }

        protected abstract void Executed(T t, DAO.DataHandler dbTarget);

        protected virtual int GetPopulationID(string cardno, DAO.DataHandler db)
        {
            if (string.IsNullOrWhiteSpace(cardno))
                return 0;

            var str = string.Format("select id from Pgis_PopulationBasicInfo where cardno = '{0}'", cardno);
            var obj = db.ExecuteScalar(str);

            return obj == null ? 0 : int.Parse(string.Format("{0}", obj));
        }

        protected virtual int CheckParam(DAO.DataHandler db, string name, string code = null, int pid = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
                return 0;

            var obj = db.ExecuteScalar(GetParamCmd(name));
            if (obj == null)
            {
                code = code ?? GetCode();
                db.ExecuteNonQuery(InsertParamCmd(pid, code, name));
                obj = db.ExecuteScalar(GetParamCmd(name));
            }

            return obj == null ? 0 : int.Parse(string.Format("{0}", obj));
        }

        protected virtual string GetParamCmd(string name)
        {
            return string.Format("select id from PGis_Param where name = '{0}'", name);
        }

        protected virtual string InsertParamCmd(int pid, string code, string name)
        {
            return string.Format("insert into PGis_Param(pid, code, name, disabled) values({0}, '{1}', '{2}', 1)", pid, code, name);
        }

        protected virtual int GetCompanyID(string token, DAO.DataHandler db)
        {
            var str = string.Format("select id from Pgis_Company where token = '{0}'", token);
            var obj = db.ExecuteScalar(str);
            return obj == null ? 0 : int.Parse(string.Format("{0}", obj));
        }

        protected virtual int GetHotelID(string token, DAO.DataHandler db)
        {
            var str = string.Format("select id from Pgis_Hotel where token = '{0}'", token);
            var obj = db.ExecuteScalar(str);
            return obj == null ? 0 : int.Parse(string.Format("{0}", obj));
        }

        protected virtual int GetSex(string code, DAO.DataHandler db, out string name)
        {
            switch (code)
            {
                case "1":
                    name = "男";
                    break;
                case "2":
                    name = "女";
                    break;
                default:
                    name = "其他";
                    break;
            }

            return CheckParam(db, name, GetCode(), 13);
        }

        protected virtual int CheckAddress(string addr, DAO.DataHandler db)
        {
            if (string.IsNullOrWhiteSpace(addr))
                return 0;

            var obj = db.ExecuteScalar(GetAddressCmd(addr));
            if (obj != null)
                return int.Parse(string.Format("{0}", obj));

            db.ExecuteNonQuery(InsertAddressCmd(addr));
            obj = db.ExecuteScalar(GetAddressCmd(addr));
            return obj == null ? 0 : int.Parse(string.Format("{0}", obj));
        }

        protected virtual int CheckArea(DAO.DataHandler db, out string outName, string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = "其它";

            outName = name;

            var obj = db.ExecuteScalar(GetAreaCmd(name));
            if (obj == null)
            {
                db.ExecuteNonQuery(InsertAreaCmd(name));
                obj = db.ExecuteScalar(GetAreaCmd(name));
            }

            return obj == null ? 0 : int.Parse(string.Format("{0}", obj));
        }

        protected virtual int CheckAdministrative(DAO.DataHandler db, out string outName, string name = "其它")
        {
            if (string.IsNullOrWhiteSpace(name))
                name = "其它";
            outName = name;

            var obj = db.ExecuteScalar(GetAdminCmd(name));
            if (obj == null)
            {
                db.ExecuteNonQuery(InsertAdminCmd(db, name));
                obj = db.ExecuteScalar(GetAdminCmd(name));
            }

            return obj == null ? 0 : int.Parse(string.Format("{0}", obj));
        }

        protected virtual string GetAdminCmd(string name)
        {
            return string.Format("select id from Pgis_Administrative where name = '{0}'", name);
        }

        protected virtual string InsertAdminCmd(DAO.DataHandler db, string name)
        {
            int areaid = 0;
            string areaname = "";
            areaid = CheckArea(db, out areaname);

            var fields = new string[] 
            {
                "Name", "Code", "PID", "FirstLetter", "AreaID", "AreaName"
            };
            var values = new string[] 
            {
                string.Format("'{0}'", name),
                string.Format("'{0}'", GetCode()),
                "'0'",
                GetFirstLetter(name),
                string.Format("'{0}'", areaid),
                string.Format("'{0}'", areaname)
            };

            return string.Format("insert into Pgis_Administrative({0}) values({1})", string.Join(",", fields), string.Join(",", values));
        }

        protected virtual string GetAreaCmd(string name)
        {
            return string.Format("select id from PGis_Area where name = '{0}'", name);
        }

        protected virtual string InsertAreaCmd(string name)
        {
            var fields = new string[] 
            {
                "pid", "name", "newcode"
            };
            var values = new string[] 
            {
                "'0'", string.Format("'{0}'", name), string.Format("'{0}'", GetCode())
            };

            return string.Format("insert into PGis_Area({0}) values({1})", string.Join(",", fields), string.Join(",", values));
        }

        protected virtual string GetAddressCmd(string addr)
        {
            return string.Format("select id from Pgis_Address where Content = '{0}'", addr);
        }

        protected virtual string InsertAddressCmd(string addr)
        {
            return string.Format("insert into Pgis_Address(Content) values('{0}')", addr);
        }

        protected string GetCode()
        {
            return string.Format("{0}", GetGuid());
        }

        protected long GetGuid()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// 获取指定名称的首字母大写组合
        /// eg. 
        ///     '首字母'->'SZM'
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected virtual string GetFirstLetter(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return "NULL";

            return string.Format("dbo.fun_getPYFirst('{0}')", str);
        }
    }
}
