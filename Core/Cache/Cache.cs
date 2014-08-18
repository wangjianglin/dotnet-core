using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
//using System.Data.OleDb;
using ADOX;
using Lin.Core.Config;
//using System.Data;
using System.Threading;
using System.Reflection;
using Lin.Core.Log;
using Lin.Core.Utils;
using Lin.Util;

namespace Lin.Core.Cache
{
    [Serializable]
    public enum Type
    {
        Obj = 0, // Object类型
        File = 1, // 文件类型
        Folder = 2 // 文件夹类型
    }

    [Serializable]
    class CacheModel
    {
        #region 属性

        /// <summary>
        /// 键
        /// </summary>
        private string key;
        public string Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        /// <summary>
        /// 值
        /// </summary>
        private string value;
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// 文件之前所在的目录
        /// </summary>
        private string oldDirectory;
        public string OldDirectory
        {
            get { return this.oldDirectory; }
            set { this.oldDirectory = value; }
        }

        /// <summary>
        /// 是否永久性缓存
        /// </summary>
        private bool isPermanency;
        public bool IsPermanency
        {
            get { return this.isPermanency; }
            set { this.isPermanency = value; }
        }

        /// <summary>
        /// 文件大小
        /// </summary>
        private string size;
        public string Size
        {
            get { return this.size; }
            set { this.size = value; }
        }

        /// <summary>
        /// 最后一次写入的时间
        /// </summary>
        private DateTime latestPutTime;
        public DateTime LatestPutTime
        {
            get { return this.latestPutTime; }
            set { this.latestPutTime = value; }
        }

        /// <summary>
        /// 最后一次获取的时间
        /// </summary>
        private DateTime latestGetTime;
        public DateTime LatestGetTime
        {
            get { return this.latestGetTime; }
            set { this.latestGetTime = value; }
        }

        #endregion
    }

    [Serializable]
    public class Cache : ICache
    {
        /// <summary>
        /// 目录
        /// </summary>
        private DirectoryInfo dir;
        public DirectoryInfo Dir
        {
            get { return this.dir; }
            private set
            {
                this.dir = value;
            }
        }

        /// <summary>
        /// 用来存放临时文件的目录
        /// </summary>
        private static DirectoryInfo _tmpDir;
        internal static DirectoryInfo TmpDir
        {
            get
            {
                _tmpDir = new DirectoryInfo(Path.GetTempPath() + "\\tmpdir");
                if (!_tmpDir.Exists)
                {
                    _tmpDir.Create();
                    _tmpDir = new DirectoryInfo(_tmpDir.FullName);
                }
                return _tmpDir;
            }
            private set
            {
                _tmpDir = value;
            }
        }

        private string DataBaseName = "\\CacheDataBase";
        private Object AsyncLock = new Object();

        internal Cache(System.IO.DirectoryInfo dir)
        {
            if (!Directory.Exists(dir.FullName))
            {
                DirectoryInfo directory = Directory.CreateDirectory(dir.FullName);
                this.dir = directory;
            }
            this.dir = dir;

            TmpDir = new DirectoryInfo(Path.GetTempPath() + "\\tmpdir");
            DeleteTmpDir(); // 删除临时文件
            CreateData(DataBaseName);
        }

        /// <summary>
        /// 根据缓存目录，查找缓存对象
        /// </summary>
        private static IDictionary<string, ICache> dict = new Dictionary<string, ICache>();
        private static object dictLock = new object();
        public static ICache GetCache(string dirName)
        {
            if (dict.ContainsKey(dirName))
            {
                return dict[dirName];
            }
            lock (dictLock)
            {
                if (dict.ContainsKey(dirName))
                {
                    return dict[dirName];
                }
                Cache cache = new Cache(new DirectoryInfo(dirName));
                dict.Add(dirName, cache);
                return cache;
            }
        }

        /// <summary>
        ///  创建一个目录用来存放一些临时并且没有用的文件，存在及删除数据，不存在创建
        /// </summary>
        private void DeleteTmpDir()
        {
            if (TmpDir.Exists)
            {
                try
                {
                    Directory.Delete(TmpDir.FullName, true);
                }
                catch
                { }
                if (!TmpDir.Exists)
                {
                    TmpDir.Create();
                }
            }
            else
            {
                Directory.CreateDirectory(TmpDir.FullName);
            }
        }

        ~Cache()
        {
            //DeleteTmpDir();
        }

        # region Access数据库的相应操作

        //private DataTable table;

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="fileName"></param>
        private void CreateData(string fileName)
        {
            //if (!Directory.Exists(dir.FullName))
            //{
            //    DirectoryInfo directory = Directory.CreateDirectory(dir.FullName);
            //}
            //fileName = this.dir.FullName + fileName;// +".mdb";
            //if (File.Exists(fileName))
            //{
            //    DataSet ds = GetDatas("");
            //    table = ds.Tables[0];
            //}
            //else
            //{
            //    string conn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName;

            //    //创建数据库
            //    ADOX.Catalog catalog = new Catalog();
            //    catalog.Create(conn);


            //    //连接数据库
            //    ADODB.Connection cn = new ADODB.Connection();
            //    cn.Open(conn, null, null, -1);
            //    catalog.ActiveConnection = cn;

            //    //新建表
            //    ADOX.Table table = new ADOX.Table();
            //    table.Name = "Cache";

            //    ADOX.Column column = new ADOX.Column();
            //    column.ParentCatalog = catalog;
            //    column.Type = ADOX.DataTypeEnum.adInteger; // 必须先设置字段类型
            //    column.Name = "ID";
            //    column.DefinedSize = 9;
            //    column.Properties["AutoIncrement"].Value = true;
            //    table.Columns.Append(column, DataTypeEnum.adInteger, 0);
            //    //设置主键
            //    table.Keys.Append("PrimaryKey", ADOX.KeyTypeEnum.adKeyPrimary, "ID", "", "");

            //    table.Columns.Append("Key", DataTypeEnum.adVarWChar, 255);
            //    table.Columns.Append("Value", DataTypeEnum.adVarWChar, 255);
            //    table.Columns.Append("CreateTime", DataTypeEnum.adDate, 0);
            //    table.Columns.Append("Type", DataTypeEnum.adInteger, 2);
            //    table.Columns.Append("Directory", DataTypeEnum.adVarWChar, 255);
            //    table.Columns.Append("IsPermanency", DataTypeEnum.adBoolean, 0);
            //    table.Columns.Append("Size", DataTypeEnum.adInteger, 255);
            //    table.Columns.Append("LatestPutTime", DataTypeEnum.adDate, 0);
            //    table.Columns.Append("LatestGetTime", DataTypeEnum.adDate, 0);

            //    catalog.Tables.Append(table);
            //    //此处一定要关闭连接，否则添加数据时候会出错

            //    table = null;
            //    catalog = null;
            //    cn.Close();
            //}
        }

        /// <summary>
        /// 获取指定Access数据库中的表的所有数据
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        //private DataSet GetDatas(string condition)
        //{
        //    string strConnection;
        //    strConnection = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + this.dir + DataBaseName + ";Persist Security Info=False";

        //    OleDbConnection conn = new OleDbConnection(strConnection);
        //    string strSql = "SELECT * From Cache WHERE 1=1";
        //    if (!string.IsNullOrEmpty(condition))
        //    {
        //        strSql += condition;
        //    }
        //    OleDbDataAdapter da = new OleDbDataAdapter(strSql, conn);
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        conn.Open();
        //    }
        //    catch
        //    {
        //        CreateData(DataBaseName);
        //    }

        //    da.Fill(ds, "Cache");
        //    da.Dispose();
        //    conn.Dispose();
        //    conn.Close();

        //    return ds;
        //}

        // 往数据库中插入数据
//        private int Insert(Type type, CacheModel cache)
//        {
//            string strConn = " Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source =" + this.dir + DataBaseName + ";Persist Security Info=False";
//            OleDbConnection myConn = new OleDbConnection(strConn);
//            try
//            {
//                myConn.Open();
//            }
//            catch
//            {
//                CreateData(DataBaseName);
//            }
//            OleDbCommand inst = new OleDbCommand(@"INSERT INTO CACHE ([Key],[Value],[CreateTime],[Type],[Directory],[IsPermanency],[Size],[LatestPutTime],[LatestGetTime]) 
//                                VALUES (@Key,@Value,@CreateTime,@Type,@Directory,@IsPermanency,@Size,@LatestPutTime,@LatestGetTime)", myConn);
//            inst.Parameters.Add("@Key", OleDbType.VarWChar, 255).Value = cache.Key;
//            inst.Parameters.Add("@Value", OleDbType.VarWChar, 255).Value = cache.Value;
//            inst.Parameters.Add("@CreateTime", OleDbType.Date, 0).Value = DateTime.Now;
//            inst.Parameters.Add("@Type", OleDbType.Integer, 2).Value = (int)type;
//            inst.Parameters.Add("@Directory", OleDbType.VarWChar, 255).Value = cache.OldDirectory;
//            inst.Parameters.Add("@IsPermanency", OleDbType.Integer, 2).Value = cache.IsPermanency;
//            inst.Parameters.Add("@Size", OleDbType.Integer, 255).Value = cache.Size;
//            inst.Parameters.Add("@LatestPutTime", OleDbType.Date, 0).Value = cache.LatestPutTime;
//            inst.Parameters.Add("@LatestGetTime", OleDbType.Date, 0).Value = cache.LatestGetTime;

//            int count = 0;
//            object obj = inst.ExecuteNonQuery();
//            if (obj != null)
//            {
//                count = Convert.ToInt32(obj);
//            }
//            myConn.Close();

//            return count;
//        }

        // 往数据库中插入数据
//        private object Update(Type type, CacheModel cache, DateTime getTime, string key)
//        {
//            object obj = new object();
//            string strConn = " Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source =" + this.dir + DataBaseName + ";Persist Security Info=False";
//            OleDbConnection myConn = new OleDbConnection(strConn);
//            try
//            {
//                myConn.Open();
//            }
//            catch
//            {
//                CreateData(DataBaseName);
//            }
//            if (cache != null && getTime == DateTime.MinValue && key == "")
//            {
//                OleDbCommand inst = new OleDbCommand(@"UPdate CACHE Set [Value] = @Value,[Type] = @Type,[Directory] = @Directory,
//                [IsPermanency] = @IsPermanency,[Size] = @Size,[LatestPutTime] = @LatestPutTime,[LatestGetTime] = @LatestGetTime WHERE [Key] = @Key", myConn);
//                inst.Parameters.Add("@Value", OleDbType.VarWChar, 255).Value = cache.Value;
//                inst.Parameters.Add("@Type", OleDbType.Integer, 2).Value = (int)type;
//                inst.Parameters.Add("@Directory", OleDbType.VarWChar, 255).Value = cache.OldDirectory;
//                inst.Parameters.Add("@IsPermanency", OleDbType.Integer, 2).Value = cache.IsPermanency;
//                inst.Parameters.Add("@Size", OleDbType.Integer, 255).Value = cache.Size;
//                inst.Parameters.Add("@LatestPutTime", OleDbType.Date, 0).Value = cache.LatestPutTime;
//                inst.Parameters.Add("@LatestGetTime", OleDbType.Date, 0).Value = cache.LatestGetTime;
//                inst.Parameters.Add("@Key", OleDbType.VarWChar, 255).Value = cache.Key;
//                obj = inst.ExecuteScalar();
//            }
//            else if (cache == null && getTime != DateTime.MinValue && !string.IsNullOrEmpty(key))
//            {
//                OleDbCommand inst = new OleDbCommand(@"UPdate CACHE Set [LatestGetTime] = @LatestGetTime WHERE [Key] = @Key", myConn);
//                inst.Parameters.Add("@LatestGetTime", OleDbType.Date, 0).Value = getTime;
//                inst.Parameters.Add("@Key", OleDbType.VarWChar, 255).Value = key;
//                obj = inst.ExecuteScalar();
//            }
//            myConn.Close();

//            return obj;
//        }

        // 删除Key相对应的数据
        //private void Delete(string key)
        //{
        //    string strConn = " Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source =" + this.dir + DataBaseName + ";Persist Security Info=False";
        //    OleDbConnection myConn = new OleDbConnection(strConn);
        //    try
        //    {
        //        myConn.Open();
        //    }
        //    catch
        //    {
        //        CreateData(DataBaseName);
        //    }
        //    OleDbCommand inst = new OleDbCommand("DELETE FROM CACHE WHERE Key = @Key", myConn);
        //    inst.Parameters.Add("@Key", OleDbType.VarWChar, 255).Value = key;
        //    inst.ExecuteScalar();
        //    myConn.Close();
        //}

        #endregion

        #region 文件操作（包括写入，获取，删除）

        #region 写入其它对象
        /// <summary>
        /// 写入Object对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="key">源键</param>
        /// <returns>产生的新键</returns>
        public string Put(object obj, string key = null, bool isPermanency = false, bool isCrossDomain = false)
        {
            if (obj == null) { return null; }
            CacheModel cacheModel = new CacheModel();
            lock (AsyncLock)
            {
                object[] atrrs = obj.GetType().GetCustomAttributes(typeof(global::System.SerializableAttribute), true); // 查找是否存在标记为Serialization的对象
                if (atrrs.Length > 0)
                {
                    try
                    {
                        if (System.IO.Directory.Exists(dir.ToString()))
                        {
                            try
                            {
                                FileInfo f = new FileInfo(this.dir.FullName + "\\ObjectFile-" + DateTime.Now.Ticks + ".tmp");

                                FileStream fs = f.Create();
                                if (key == null)
                                {
                                    key = "key-" + DateTime.Now.Ticks;
                                }
                                cacheModel.Value = f.Name;
                                cacheModel.Key = key;
                                cacheModel.OldDirectory = "";
                                cacheModel.LatestPutTime = DateTime.Now;
                                cacheModel.IsPermanency = isPermanency;
                                cacheModel.Size = f.Length.ToString();

                                key = OperationDB(Type.Obj, cacheModel); // 数据库操作
                                BinaryFormatter sl = new BinaryFormatter();
                                sl.Serialize(fs, obj);
                                fs.Close();
                                System.Threading.Thread.Sleep(1);
                            }
                            catch (Exception ex)
                            {
                                //Lin.Core.Controls.TaskbarNotifierUtil.Show(new AdException(-0x2002002, ex));
                            }
                        }
                        else
                        {
                            Lin.Core.Controls.TaskbarNotifierUtil.Show("读取的文件对象不存在", LogLevel.INFO, "温馨提示");
                        }
                    }
                    catch (Exception ex)
                    {
                        Lin.Core.Controls.TaskbarNotifierUtil.Show(new LinException(-0x2002003, ex));
                    }
                }
                return key;
            }
        }
        #endregion

        #region 写入文件信息
        /// <summary>
        /// 写入文件信息
        /// </summary>_
        /// <param name="file">文件对象</param>
        /// <param name="key">传过来的键</param>
        /// <returns>新的键</returns>
        public string Put(FileInfo file, string key = null, bool isPermanency = false)
        {
            if (file == null) { return null; }
            CacheModel cacheModel = new CacheModel();
            lock (AsyncLock)
            {
                if (!System.IO.Directory.Exists(dir.ToString()) || !File.Exists(file.FullName))
                {
                    return null;
                }
                else
                {
                    try
                    {
                        FileInfo f = new FileInfo(this.dir.FullName + "\\FileInfo-" + DateTime.Now.Ticks + file.Extension);
                        //FileInfo f = new FileInfo(this.dir.FullName + "\\FileInfo-" + DateTime.Now.Ticks);
                        file.CopyTo(Path.Combine(dir.FullName, f.Name), true); // 复制文件
                        if (key == null)
                        {
                            key = "key-" + DateTime.Now.Ticks;
                        }
                        cacheModel.OldDirectory = file.DirectoryName;
                        cacheModel.Value = f.Name;
                        cacheModel.Key = key;
                        cacheModel.IsPermanency = isPermanency;
                        cacheModel.Size = f.Length.ToString();

                        key = OperationDB(Type.File, cacheModel);
                    }
                    catch (Exception ex)
                    {
                        Lin.Core.Controls.TaskbarNotifierUtil.Show(new LinException(-0x2002004, ex));
                    }

                    return key;
                }
            }
        }
        #endregion

        #region 写入文件夹

        /// <summary>
        /// 写入文件夹
        /// </summary>
        /// <param name="directory">文件目录</param>
        /// <param name="key">传过来的键</param>
        /// <param name="isPermanency">上方永久缓存</param>
        /// <param name="isRecursive">是否复制文件目录下的子目录</param>
        /// <returns>返回服务器上文件夹相对应的Key值</returns>
        public string Put(DirectoryInfo directory, string key = null, bool isPermanency = false, bool isRecursive = false)
        {
            if (directory == null || string.IsNullOrEmpty(directory.FullName)) { return null; }
            CacheModel cacheModel = new CacheModel();
            lock (AsyncLock)
            {
                // 被缓存的目录是否在缓存目录的tmpDir目录下
                if (!directory.FullName.StartsWith(TmpDir.FullName + @"\"))
                {
                    //被缓存的目录在缓存的目录之下
                    if (directory.FullName.StartsWith(this.dir.FullName + @"\"))
                    {
                        return null;
                    }
                    // 缓存的目录在被缓存的目录下
                    if (this.dir.FullName.StartsWith(directory.FullName + @"\"))
                    {
                        return null;
                    }
                }
                if (!directory.Exists)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        // 从一个目录将其内容复制到另一目录
                        string value = "DirectoryInfo-" + DateTime.Now.Ticks + "\\" + directory.Name;
                        CopyFolderTo(Path.Combine(directory.FullName), Path.Combine(dir.FullName + "\\" + value), isRecursive);
                        if (key == null)
                        {
                            key = "key-" + DateTime.Now.Ticks;
                        }
                        cacheModel.Value = value;
                        cacheModel.Key = key;
                        cacheModel.OldDirectory = directory.FullName;
                        cacheModel.IsPermanency = isPermanency;
                        cacheModel.Size = (DirSize(directory) / 1024).ToString();

                        key = OperationDB(Type.Folder, cacheModel);
                    }
                    catch (Exception e)
                    {
                        Lin.Core.Controls.TaskbarNotifierUtil.Show(new LinException(-0x2002005, e));
                    }
                    return key;
                }
            }
        }

        /// <summary>  
        /// 从一个目录将其内容复制到另一目录  
        /// </summary>  
        /// <param name="directorySource">源目录</param>  
        /// <param name="directoryTarget">目的目录</param>  
        private void CopyFolderTo(string directorySource, string directoryTarget, bool isRecursive)
        {
            lock (AsyncLock)
            {
                // 检查是否存在目的目录  
                if (!Directory.Exists(directoryTarget))
                {
                    Directory.CreateDirectory(directoryTarget);
                }
                // 先来复制文件  
                DirectoryInfo directoryInfo = new DirectoryInfo(directorySource);
                FileInfo[] files = directoryInfo.GetFiles();
                // 复制所有文件  
                foreach (FileInfo file in files)
                {
                    file.CopyTo(Path.Combine(directoryTarget, file.Name));
                }
                 
                if (isRecursive)
                {
                    // 最后复制目录 
                    DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
                    foreach (DirectoryInfo dir in directoryInfoArray)
                    {
                        CopyFolderTo(Path.Combine(directorySource, dir.Name), Path.Combine(directoryTarget, dir.Name), isRecursive);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 通过Key查找相对应的文件
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public object Get(string key)
        {
            //if (string.IsNullOrEmpty(key)) { return null; }
            //lock (AsyncLock)
            //{
            //    object obj = null;
            //    int num = 0;
            //    string sql = " AND Key = '" + key + "'";
            //    table = GetDatas(sql).Tables[0];
            //    FileInfo[] files = this.dir.GetFiles();
            //    //DirectoryInfo[] dirs = this.dir.GetDirectories();
            //    // 数据库中存在，目录中不存在，删除数据库中的数据
            //    foreach (DataRow row in table.Rows)
            //    {
            //        string value = row["Value"].ToString();
            //        if (key == row["Key"].ToString())
            //        {
            //            if ((Type)Convert.ToInt32(row["Type"]) == Type.File)
            //            {
            //                obj = null;
            //                foreach (FileInfo file in files)
            //                {
            //                    if (value == file.Name)
            //                    {
            //                        if (File.Exists(this.dir.FullName + "\\" + value))
            //                        {
            //                            obj = file;
            //                            Update(Type.File, null, DateTime.Now, key);
            //                        }
            //                        else
            //                        {
            //                            Delete(key);
            //                        }
            //                    }
            //                }
            //            }
            //            else if ((Type)Convert.ToInt32(row["Type"]) == Type.Folder)
            //            {
            //                obj = null;

            //                DirectoryInfo tmpDir = new DirectoryInfo(this.dir.FullName + "\\" + value);
            //                if (tmpDir.Exists)
            //                {
            //                    obj = tmpDir;
            //                }
            //                else
            //                {
            //                    Delete(key);
            //                }
            //            }
            //            else
            //            {
            //                obj = new object();
            //                obj = row["Value"];
            //                int start = row["Value"].ToString().IndexOf("-");
            //                int end = row["Value"].ToString().IndexOf(".");
            //                if (File.Exists(this.dir.FullName + "\\" + obj))
            //                {
            //                    // 反序列化
            //                    try
            //                    {
            //                        FileStream fileStream = new FileStream(this.dir.FullName + "\\" + obj, FileMode.Open, FileAccess.Read, FileShare.Read);
            //                        BinaryFormatter bf = new BinaryFormatter();
            //                        obj = bf.Deserialize(fileStream) as object;
            //                        fileStream.Close();
            //                    }
            //                    catch (Exception e)
            //                    {
            //                        //Lin.Core.Controls.TaskbarNotifierUtil.Show("序列化失败!",Log.Level.WARNING);
            //                    }

            //                }
            //                else
            //                {
            //                    Delete(key);
            //                }
            //            }
            //            num++;
            //        }
            //    }
            //    // 如果数据库中不存在，缓存目录下存在，删除目录下的文件
            //    if (num < 1)
            //    {
            //        Remove(key);
            //    }

            //    return obj;
            //}
            return null;
        }

        /// <summary>
        /// 删除指定的Key所对应的文件(文件夹以及数据库中的数据)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void Remove(string key)
        {
            //if (string.IsNullOrEmpty(key)) { return; }
            //lock (AsyncLock)
            //{

            //    // 通过Key获取相对应的值
            //    string sql = " AND Key = '" + key + "'";
            //    table = GetDatas(sql).Tables[0];
            //    if (table.Rows.Count > 0)
            //    {
            //        DataRow row = table.Rows[0];
            //        string value = row["Value"].ToString();
            //        Type type = (Type)Convert.ToInt32(row["Type"]);

            //        FileInfo[] files = this.dir.GetFiles();
            //        DirectoryInfo[] dirs = this.dir.GetDirectories();
            //        if (type == Type.File || type == Type.Obj)
            //        {
            //            foreach (FileInfo file in files)
            //            {
            //                if (file.Name == value)
            //                {
            //                    file.Delete();
            //                }
            //            }
            //        }
            //        else if (type == Type.Folder)
            //        {
            //            foreach (DirectoryInfo dir in dirs)
            //            {
            //                if (value.Substring(0, value.IndexOf(@"\")) == dir.Name)
            //                {
            //                    dir.Delete(true);
            //                }
            //            }
            //        }

            //        this.Delete(key);
            //    }
            //}
        }

        /// <summary>
        /// 对不同类型的文件进行数据库操作
        /// </summary>
        /// <param name="type">类型</param>
        private string OperationDB(Type type, CacheModel cacheModel)
        {
            //int count = 0;
            //DataSet ds = GetDatas("");
            //table = ds.Tables[0];
            //// 判断是否存在该Key，如果存在，则删除目录下之前的文件，增加现在Key所对应的文件；不存在，直接新增文件
            //if (table.Rows.Count < 1)
            //{
            //    cacheModel.LatestPutTime = DateTime.Now;
            //    Insert(type, cacheModel);
            //}
            //else
            //{
            //    foreach (DataRow row in table.Rows)
            //    {
            //        if (cacheModel.Key == row["Key"].ToString())
            //        {
            //            try
            //            {
            //                if (type == Type.File || type == Type.Obj)
            //                {
            //                    File.Delete(this.dir + "\\" + row["Value"].ToString());
            //                }
            //                else
            //                {
            //                    Directory.Delete(this.dir + "\\" + row["Value"].ToString(), true);
            //                    int index = row["Value"].ToString().IndexOf('\\');
            //                    Directory.Delete(this.dir + "\\" + row["Value"].ToString().Substring(0, index));
            //                }
            //                cacheModel.LatestPutTime = DateTime.Now;
            //                Update(type, cacheModel, DateTime.MinValue, "");
            //                count++;
            //            }
            //            catch (Exception ex)
            //            {
            //                Lin.Core.Controls.TaskbarNotifierUtil.Show(new LinException(-0x2002006, ex));
            //            }
            //            break;
            //        }
            //    }
            //    if (count <= 0)
            //    {
            //        cacheModel.LatestPutTime = DateTime.Now;
            //        Insert(type, cacheModel);
            //    }
            //}

            //return cacheModel.Key;
            return null;
        }

        #endregion

        /// <summary>
        /// 删除路径下比数据库中存储多余的文件以及文件夹
        /// </summary>
        public void DeleteRemain()
        {
            //lock (AsyncLock)
            //{
            //    DataTable table = GetDatas("").Tables[0];
            //    FileInfo[] files = this.dir.GetFiles();
            //    DirectoryInfo[] directoryInfoArray = this.dir.GetDirectories();

            //    IList<string> fileValues = new List<string>();
            //    IList<string> folderValues = new List<string>();

            //    foreach (DataRow row in table.Rows)
            //    {
            //        if ((Type)Convert.ToInt32(row["Type"]) == Type.File)
            //        {
            //            fileValues.Add(row["Value"].ToString());
            //        }
            //        else if ((Type)Convert.ToInt32(row["Type"]) == Type.Folder)
            //        {
            //            string value = row["Value"].ToString();
            //            folderValues.Add(value.Substring(0, value.IndexOf(@"\")));
            //        }
            //    }
            //    // 删除文件中存在但是数据库中不存在的文件
            //    foreach (FileInfo file in files)
            //    {
            //        if (!fileValues.Contains(file.Name) && File.Exists(this.dir.FullName + file.Name))
            //        {
            //            file.Delete();
            //        }
            //    }

            //    // 删除目录下多余的文件夹以及文件夹下所有的文件
            //    foreach (DirectoryInfo directory in directoryInfoArray)
            //    {
            //        if (!folderValues.Contains(directory.Name) && Directory.Exists(directory.FullName))
            //        {
            //            directory.Delete(true);
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 用户更改缓存目录后，将之前缓存目录下的内容移动到新的缓存目录
        /// </summary>
        /// <param name="directorySource">旧目录</param>
        /// <param name="directoryTarget">新的缓存目录</param>
        public void MoveFolderTo(DirectoryInfo directorySource, DirectoryInfo directoryTarget)
        {
            //bool flag = true;
            //lock (AsyncLock)
            //{
            //    // 先来移动文件  
            //    FileInfo[] files = directorySource.GetFiles();
            //    DataTable dtFile = this.GetDatas(" AND (Type =" + (int)(Type.File) + " OR Type =" + (int)(Type.Obj) + ")").Tables[0];
            //    DataTable dtDir = this.GetDatas(" AND Type =" + (int)Type.Folder).Tables[0];
            //    // 检查是否存在目的目录  
            //    if (!Directory.Exists(directoryTarget.FullName))
            //    {
            //        Directory.CreateDirectory(directoryTarget.FullName);
            //    }
            //    if (directoryTarget.FullName.StartsWith(directorySource.FullName + @"\"))
            //    {
            //        if (dtDir != null && dtDir.Rows.Count > 0)
            //        {
            //            foreach (DataRow row in dtDir.Rows)
            //            {
            //                string[] str = row["Value"].ToString().Split('\\');
            //                if ((directoryTarget.Name == str[1] && directoryTarget.Parent.Name == str[0]) || (directoryTarget.Name == str[0]))
            //                {
            //                    flag = false;
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //    if (!flag)
            //    {
            //        throw new Exception(directoryTarget.FullName + "  目录为现有缓存目录中的缓存文件夹,不能再作为缓存目录，请重新选择！");
            //    }
            //    CopyCacheFiles(directorySource, directoryTarget, files, dtFile); // 移动缓存文件

            //    // 最后移动目录  
            //    DirectoryInfo[] directoryInfoArray = directorySource.GetDirectories();
            //    if (directoryInfoArray.Length > 0)
            //    {
            //        foreach (DirectoryInfo dir in directoryInfoArray)
            //        {
            //            if (dtDir != null && dtDir.Rows.Count > 0)
            //            {
            //                foreach (DataRow row in dtDir.Rows)
            //                {
            //                    string[] str = row["Value"].ToString().Split('\\');
            //                    DirectoryInfo dirTarget = new DirectoryInfo(directoryTarget.FullName + "\\" + dir.Name);
            //                    DirectoryInfo dirSource = new DirectoryInfo(directorySource.FullName + "\\" + dir.Name);
            //                    if (dir.Name == str[0])
            //                    {
            //                        try
            //                        {
            //                            if (!Directory.Exists(directorySource.FullName))
            //                                return;
            //                            CopyDir(dirSource.FullName, dirTarget.FullName);
            //                            Directory.Delete(dirSource.FullName);
            //                        }
            //                        catch (Exception ex)
            //                        {
            //                            Directory.Delete(dirTarget.FullName, true);
            //                            MoveFolderTo(directoryTarget, directorySource);
            //                            ILogger log = Logger.Logs["system.cache.exception"];
            //                            log.LogRecord(ExceptionInfoToString.ADExceptionToString(new LinException(-0x2002008, ex)));
            //                            throw new Exception("更改缓存目录失败，可能是选择的缓存目录过长");
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 移动缓存文件
        /// </summary>
        /// <param name="directorySource">源目录</param>
        /// <param name="directoryTarget">目标目录</param>
        /// <param name="files">所需移动的文件</param>
        /// <param name="dtFile">数据中存在的文件标识</param>
        //private void CopyCacheFiles(DirectoryInfo directorySource, DirectoryInfo directoryTarget, FileInfo[] files, DataTable dtFile)
        //{
        //    // 移动所有文件  
        //    foreach (FileInfo file in files)
        //    {
        //        if (("\\" + file.Name) == DataBaseName)
        //        {
        //            try
        //            {
        //                file.CopyTo(Path.Combine(directoryTarget.FullName, file.Name), true);
        //            }
        //            catch
        //            {
        //                continue;
        //            }
        //            // 移动后删除文件
        //            try
        //            {
        //                FileInfo oldFile = new FileInfo(directorySource.FullName + "\\" + file.Name);
        //                oldFile.Delete();
        //            }
        //            catch (Exception ex)
        //            {
        //                Lin.Core.Controls.TaskbarNotifierUtil.Show(new LinException(-0x2002007, ex));
        //            }
        //        }
        //        else
        //        {
        //            if (dtFile != null && dtFile.Rows.Count > 0)
        //            {
        //                foreach (DataRow row in dtFile.Rows)
        //                {
        //                    if (file.Name == row["Value"].ToString())
        //                    {
        //                        try
        //                        {
        //                            file.CopyTo(Path.Combine(directoryTarget.FullName, file.Name), true);
        //                            // 移动后删除文件
        //                            try
        //                            {
        //                                FileInfo oldFile = new FileInfo(directorySource.FullName + "\\" + file.Name);
        //                                oldFile.Delete();
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                Lin.Core.Controls.TaskbarNotifierUtil.Show(new LinException(-0x2002007, ex));
        //                            }
        //                            break;
        //                        }
        //                        catch
        //                        {
        //                            continue;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        this.dir = directoryTarget;
        //    }
        //}

        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="fromDir">旧文件夹</param>
        /// <param name="toDir">新文件夹</param>
        private static void CopyDir(string fromDir, string toDir)
        {
            if (!Directory.Exists(fromDir))
                return;

            if (!Directory.Exists(toDir))
            {
                Directory.CreateDirectory(toDir);
            }

            string[] files = Directory.GetFiles(fromDir);
            foreach (string fromFileName in files)
            {
                string fileName = Path.GetFileName(fromFileName);
                string toFileName = Path.Combine(toDir, fileName);
                File.Copy(fromFileName, toFileName);
            }
            string[] fromDirs = Directory.GetDirectories(fromDir);
            foreach (string fromDirName in fromDirs)
            {
                string dirName = Path.GetFileName(fromDirName);
                string toDirName = Path.Combine(toDir, dirName);
                CopyDir(fromDirName, toDirName);
                Directory.Delete(fromDirName, true);
            }
        }

        /// <summary>
        /// 当缓存文件夹的大小已经超过规定大小时，淘汰最旧不是永久的缓存文件
        /// </summary>
        /// <param name="directorySize">缓存目录大小</param>
        public void EliminationSpilthFile(long directorySize)
        {
            //lock (AsyncLock)
            //{
            //    long length = GetInfoSize(); // 获取缓存目录的所占用的大小
            //    string sql = " AND IsPermanency = 0 ORDER BY CreateTime ASC";
            //    table = this.GetDatas(sql).Tables[0];
            //    if (table.Rows.Count > 0)
            //    {
            //        DataRow row = table.Rows[0];
            //        while (length >= directorySize)
            //        {
            //            if (row["Key"] != null)
            //            {
            //                Remove(row["Key"].ToString());
            //                length = GetInfoSize();
            //                table = this.GetDatas(sql).Tables[0];
            //                if (table.Rows.Count > 0)
            //                {
            //                    row = table.Rows[0];
            //                }
            //            }
            //        };
            //    }
            //    //DeleteRemain();
            //}
        }

        #region 计算文件夹大小

        private long GetInfoSize()
        {
            //long Size = 0;
            //DataTable dt = GetDatas("").Tables[0];
            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    foreach (DataRow row in dt.Rows)
            //    {
            //        if (row != null)
            //        {
            //            if ((Type)row["Type"] == Type.File)
            //            {
            //                Size += Int64.Parse(row["Size"].ToString()) / 1024;
            //            }
            //            else if ((Type)row["Type"] == Type.Folder)
            //            {
            //                Size += Int64.Parse(row["Size"].ToString());
            //            }
            //        }
            //    }
            //}
            //return (Size);
            return 0;
        }

        private long DirSize(DirectoryInfo dir)
        {
            long Size = 0;
            FileInfo[] fis = dir.GetFiles();
            foreach (FileInfo fi in fis)
            {
                Size += fi.Length;
            }
            DirectoryInfo[] dis = dir.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                Size += DirSize(di);
            }
            return (Size);
        }

        #endregion

        private System.Threading.Thread thread = null;
        private UInt64 clearCacheTimeSpan = 0;
        private long clearCacheSize = 0;
        private object timelyClearCacheObject = new object();
        /// <summary>
        /// 定时清理缓存
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="size"></param>
        public void TimelyClearCache(UInt64 timeSpan, long size)
        {
            this.clearCacheTimeSpan = timeSpan;
            this.clearCacheSize = size;
            if (thread == null)
            {
                lock (timelyClearCacheObject)
                {
                    if (thread == null)
                    {
                        thread = Lin.Core.Thread.BackThread(obj =>
                        {
                            while (true)
                            {
                                if (clearCacheTimeSpan == 0)
                                {
                                    System.Threading.Thread.Sleep(10);
                                }
                                else
                                {
                                    try
                                    {
                                        // 清理缓存
                                        EliminationSpilthFile(clearCacheSize);
                                    }
                                    catch (Exception ex)
                                    {
                                        Lin.Core.Controls.TaskbarNotifierUtil.Show(new LinException(-0x2002009, ex));
                                    }
                                    UInt64 N = (((UInt64)clearCacheTimeSpan) * 1000) / (UInt64)int.MaxValue + 1;
                                    for (UInt64 n = 0; n < N; n++)
                                    {
                                        if (n < N - 1)
                                        {
                                            System.Threading.Thread.Sleep(int.MaxValue);
                                        }
                                        else
                                        {
                                            System.Threading.Thread.Sleep(((int)((((UInt64)clearCacheTimeSpan) * 1000) % (UInt64)int.MaxValue)));
                                        }
                                    }
                                }
                            }
                        }, null);
                    }
                }
            }
        }
    }
}
