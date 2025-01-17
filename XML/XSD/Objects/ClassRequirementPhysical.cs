// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by Xsd2Code++. Version 6.0.22.0. www.xsd2code.com
//  </auto-generated>
// ------------------------------------------------------------------------------
#pragma warning disable
namespace Hybrasyl.Xml
{
using System;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Collections;
using System.Xml.Schema;
using System.ComponentModel;
using System.Xml;
using System.IO;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4161.0")]
[Serializable]
[DebuggerStepThrough]
[DesignerCategoryAttribute("code")]
[XmlTypeAttribute(AnonymousType=true, Namespace="http://www.hybrasyl.com/XML/Hybrasyl/2020-02")]
public partial class ClassRequirementPhysical
{
    #region Private fields
    private byte _str;
    private byte _int;
    private byte _wis;
    private byte _con;
    private byte _dex;
    private uint _hp;
    private uint _mp;
    private static XmlSerializer _serializerXml;
    #endregion
    
    public ClassRequirementPhysical()
    {
        _str = ((byte)(0));
        _int = ((byte)(0));
        _wis = ((byte)(0));
        _con = ((byte)(0));
        _dex = ((byte)(0));
        _hp = ((uint)(0));
        _mp = ((uint)(0));
    }
    
    [XmlAttribute]
    [DefaultValue(typeof(byte), "0")]
    public byte Str
    {
        get
        {
            return _str;
        }
        set
        {
            _str = value;
        }
    }
    
    [XmlAttribute]
    [DefaultValue(typeof(byte), "0")]
    public byte Int
    {
        get
        {
            return _int;
        }
        set
        {
            _int = value;
        }
    }
    
    [XmlAttribute]
    [DefaultValue(typeof(byte), "0")]
    public byte Wis
    {
        get
        {
            return _wis;
        }
        set
        {
            _wis = value;
        }
    }
    
    [XmlAttribute]
    [DefaultValue(typeof(byte), "0")]
    public byte Con
    {
        get
        {
            return _con;
        }
        set
        {
            _con = value;
        }
    }
    
    [XmlAttribute]
    [DefaultValue(typeof(byte), "0")]
    public byte Dex
    {
        get
        {
            return _dex;
        }
        set
        {
            _dex = value;
        }
    }
    
    [XmlAttribute]
    [DefaultValue(typeof(uint), "0")]
    public uint Hp
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = value;
        }
    }
    
    [XmlAttribute]
    [DefaultValue(typeof(uint), "0")]
    public uint Mp
    {
        get
        {
            return _mp;
        }
        set
        {
            _mp = value;
        }
    }
    
    private static XmlSerializer SerializerXml
    {
        get
        {
            if ((_serializerXml == null))
            {
                _serializerXml = new XmlSerializerFactory().CreateSerializer(typeof(ClassRequirementPhysical));
            }
            return _serializerXml;
        }
    }
    
    #region Serialize/Deserialize
    /// <summary>
    /// Serialize ClassRequirementPhysical object
    /// </summary>
    /// <returns>XML value</returns>
    public virtual string Serialize()
    {
        StreamReader streamReader = null;
        MemoryStream memoryStream = null;
        try
        {
            memoryStream = new MemoryStream();
            System.Xml.XmlWriterSettings xmlWriterSettings = new System.Xml.XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.IndentChars = "  ";
            System.Xml.XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
            SerializerXml.Serialize(xmlWriter, this);
            memoryStream.Seek(0, SeekOrigin.Begin);
            streamReader = new StreamReader(memoryStream);
            return streamReader.ReadToEnd();
        }
        finally
        {
            if ((streamReader != null))
            {
                streamReader.Dispose();
            }
            if ((memoryStream != null))
            {
                memoryStream.Dispose();
            }
        }
    }
    
    /// <summary>
    /// Deserializes ClassRequirementPhysical object
    /// </summary>
    /// <param name="input">string to deserialize</param>
    /// <param name="obj">Output ClassRequirementPhysical object</param>
    /// <param name="exception">output Exception value if deserialize failed</param>
    /// <returns>true if this Serializer can deserialize the object; otherwise, false</returns>
    public static bool Deserialize(string input, out ClassRequirementPhysical obj, out Exception exception)
    {
        exception = null;
        obj = default(ClassRequirementPhysical);
        try
        {
            obj = Deserialize(input);
            return true;
        }
        catch (Exception ex)
        {
            exception = ex;
            return false;
        }
    }
    
    public static bool Deserialize(string input, out ClassRequirementPhysical obj)
    {
        Exception exception = null;
        return Deserialize(input, out obj, out exception);
    }
    
    public static ClassRequirementPhysical Deserialize(string input)
    {
        StringReader stringReader = null;
        try
        {
            stringReader = new StringReader(input);
            return ((ClassRequirementPhysical)(SerializerXml.Deserialize(XmlReader.Create(stringReader))));
        }
        finally
        {
            if ((stringReader != null))
            {
                stringReader.Dispose();
            }
        }
    }
    
    public static ClassRequirementPhysical Deserialize(Stream s)
    {
        return ((ClassRequirementPhysical)(SerializerXml.Deserialize(s)));
    }
    #endregion
    
    /// <summary>
    /// Serializes current ClassRequirementPhysical object into file
    /// </summary>
    /// <param name="fileName">full path of outupt xml file</param>
    /// <param name="exception">output Exception value if failed</param>
    /// <returns>true if can serialize and save into file; otherwise, false</returns>
    public virtual bool SaveToFile(string fileName, out Exception exception)
    {
        exception = null;
        try
        {
            SaveToFile(fileName);
            return true;
        }
        catch (Exception e)
        {
            exception = e;
            return false;
        }
    }
    
    public virtual void SaveToFile(string fileName)
    {
        StreamWriter streamWriter = null;
        try
        {
            string dataString = Serialize();
            FileInfo outputFile = new FileInfo(fileName);
            streamWriter = outputFile.CreateText();
            streamWriter.WriteLine(dataString);
            streamWriter.Close();
        }
        finally
        {
            if ((streamWriter != null))
            {
                streamWriter.Dispose();
            }
        }
    }
    
    /// <summary>
    /// Deserializes xml markup from file into an ClassRequirementPhysical object
    /// </summary>
    /// <param name="fileName">File to load and deserialize</param>
    /// <param name="obj">Output ClassRequirementPhysical object</param>
    /// <param name="exception">output Exception value if deserialize failed</param>
    /// <returns>true if this Serializer can deserialize the object; otherwise, false</returns>
    public static bool LoadFromFile(string fileName, out ClassRequirementPhysical obj, out Exception exception)
    {
        exception = null;
        obj = default(ClassRequirementPhysical);
        try
        {
            obj = LoadFromFile(fileName);
            return true;
        }
        catch (Exception ex)
        {
            exception = ex;
            return false;
        }
    }
    
    public static bool LoadFromFile(string fileName, out ClassRequirementPhysical obj)
    {
        Exception exception = null;
        return LoadFromFile(fileName, out obj, out exception);
    }
    
    public static ClassRequirementPhysical LoadFromFile(string fileName)
    {
        FileStream file = null;
        StreamReader sr = null;
        try
        {
            file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            sr = new StreamReader(file);
            string dataString = sr.ReadToEnd();
            sr.Close();
            file.Close();
            return Deserialize(dataString);
        }
        finally
        {
            if ((file != null))
            {
                file.Dispose();
            }
            if ((sr != null))
            {
                sr.Dispose();
            }
        }
    }
}
}
#pragma warning restore
