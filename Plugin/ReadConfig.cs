using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Lin.Plugin
{
    /// <summary>
    /// 读取XML配置信息类
    /// </summary>
    public class ReadConfig
    {
        public ReadConfig(FileInfo xmlFile)
        {
            this.xmlFile = xmlFile;
            ReadXmlInformation();
        }
        /// <summary>
        /// XML文件
        /// </summary>
        private FileInfo xmlFile;
        /// <summary>
        /// 读取XML文件的配置信息
        /// </summary>
        private void ReadXmlInformation()
        {
            XDocument xml = XDocument.Load(xmlFile.FullName);
            IEnumerable<XElement> elements = xml.Descendants("IsLoadSystemCoreDir");
            foreach (XElement element in elements)
            {
                string isauto = element.Value;
                if (isauto.ToLower().Trim() == "true")
                {
                    this.IsLoadSystemCoreDir = true;
                    break;
                }
                else
                {
                    this.IsLoadSystemCoreDir = false;
                    break;
                }
            }
            elements = xml.Descendants("LoadSystemCoreDirVersion");
            foreach (XElement item in elements)
            {
                this.LoadSystemCoreDirVersion = item.Value;
                break;
            }
            this.IsCreatNewDomain = true;
            elements = xml.Descendants("IsCreatNewDomain");
            foreach (XElement item in elements)
            {
                string iscreat = item.Value;
                if (iscreat.ToLower().Trim() == "true")
                {
                    this.IsCreatNewDomain = true;
                    break;
                }
                else
                {
                    this.IsCreatNewDomain = false;
                    break;
                }
            }
        }
        /// <summary>
        /// 获取当前目录的加载是否需要加载系统核心DLL
        /// </summary>
        /// <returns></returns>
        public bool IsLoadSystemCoreDir { get; private set; }
        /// <summary>
        /// 获取需要加载系统核心目录的版本号
        /// </summary>
        /// <returns></returns>
        public string LoadSystemCoreDirVersion { get; private set; }
        /// <summary>
        /// 是否需要创建新的应用程序域加载该插件
        /// </summary>
        public bool IsCreatNewDomain { get; private set; }

        /// <summary>
        /// 获取当前目录的加载是否需要加载系统核心DLL
        /// </summary>
        /// <returns></returns>
        //public bool IsLoadSystemCoreDir()
        //{
        //    XDocument xml = XDocument.Load(xmlFile.FullName);
        //    IEnumerable<XElement> elements = xml.Descendants("IsLoadSystemCoreDir");
        //    foreach (XElement element in elements)
        //    {
        //        string isauto = element.Value;
        //        if (isauto.ToLower().Trim() == "true")
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    return false;
        //}
        /// <summary>
        /// 获取需要加载系统核心目录的版本号
        /// </summary>
        /// <returns></returns>
        //public string LoadSystemCoreDirVersion()
        //{
        //    XDocument xml = XDocument.Load(xmlFile.FullName);
        //    IEnumerable<XElement> elements = xml.Descendants("LoadSystemCoreDirVersion");
        //    foreach (XElement item in elements)
        //    {
        //        return item.Value;
        //    }
        //    return null;
        //}
    }
}
