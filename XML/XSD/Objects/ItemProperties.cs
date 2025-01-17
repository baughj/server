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
[XmlTypeAttribute(Namespace="http://www.hybrasyl.com/XML/Hybrasyl/2020-02")]
public partial class ItemProperties
{
    #region Private fields
    private Appearance _appearance;
    private List<CastModifier> _castModifiers;
    private Stackable _stackable;
    private Physical _physical;
    private List<Category> _categories;
    private Equipment _equipment;
    private StatModifiers _statModifiers;
    private ItemFlags _flags;
    private Variants _variants;
    private Vendor _vendor;
    private ItemDamage _damage;
    private Use _use;
    private ItemRestrictions _restrictions;
    private List<ItemMotion> _motions;
    private static XmlSerializer _serializerXml;
    #endregion
    
    public ItemProperties()
    {
        _physical = new Physical();
        _stackable = new Stackable();
        _appearance = new Appearance();
    }
    
    public Appearance Appearance
    {
        get
        {
            return _appearance;
        }
        set
        {
            _appearance = value;
        }
    }
    
    [XmlArrayItemAttribute("Match", IsNullable=false)]
    public List<CastModifier> CastModifiers
    {
        get
        {
            return _castModifiers;
        }
        set
        {
            _castModifiers = value;
        }
    }
    
    public Stackable Stackable
    {
        get
        {
            return _stackable;
        }
        set
        {
            _stackable = value;
        }
    }
    
    public Physical Physical
    {
        get
        {
            return _physical;
        }
        set
        {
            _physical = value;
        }
    }
    
    [XmlArrayItemAttribute(IsNullable=false)]
    public List<Category> Categories
    {
        get
        {
            return _categories;
        }
        set
        {
            _categories = value;
        }
    }
    
    public Equipment Equipment
    {
        get
        {
            return _equipment;
        }
        set
        {
            _equipment = value;
        }
    }
    
    public StatModifiers StatModifiers
    {
        get
        {
            return _statModifiers;
        }
        set
        {
            _statModifiers = value;
        }
    }
    
    public ItemFlags Flags
    {
        get
        {
            return _flags;
        }
        set
        {
            _flags = value;
        }
    }
    
    public Variants Variants
    {
        get
        {
            return _variants;
        }
        set
        {
            _variants = value;
        }
    }
    
    public Vendor Vendor
    {
        get
        {
            return _vendor;
        }
        set
        {
            _vendor = value;
        }
    }
    
    public ItemDamage Damage
    {
        get
        {
            return _damage;
        }
        set
        {
            _damage = value;
        }
    }
    
    public Use Use
    {
        get
        {
            return _use;
        }
        set
        {
            _use = value;
        }
    }
    
    public ItemRestrictions Restrictions
    {
        get
        {
            return _restrictions;
        }
        set
        {
            _restrictions = value;
        }
    }
    
    [XmlArrayItemAttribute("Motion", IsNullable=false)]
    public List<ItemMotion> Motions
    {
        get
        {
            return _motions;
        }
        set
        {
            _motions = value;
        }
    }
    
    private static XmlSerializer SerializerXml
    {
        get
        {
            if ((_serializerXml == null))
            {
                _serializerXml = new XmlSerializerFactory().CreateSerializer(typeof(ItemProperties));
            }
            return _serializerXml;
        }
    }
    
    #region Serialize/Deserialize
    /// <summary>
    /// Serialize ItemProperties object
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
    /// Deserializes ItemProperties object
    /// </summary>
    /// <param name="input">string to deserialize</param>
    /// <param name="obj">Output ItemProperties object</param>
    /// <param name="exception">output Exception value if deserialize failed</param>
    /// <returns>true if this Serializer can deserialize the object; otherwise, false</returns>
    public static bool Deserialize(string input, out ItemProperties obj, out Exception exception)
    {
        exception = null;
        obj = default(ItemProperties);
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
    
    public static bool Deserialize(string input, out ItemProperties obj)
    {
        Exception exception = null;
        return Deserialize(input, out obj, out exception);
    }
    
    public static ItemProperties Deserialize(string input)
    {
        StringReader stringReader = null;
        try
        {
            stringReader = new StringReader(input);
            return ((ItemProperties)(SerializerXml.Deserialize(XmlReader.Create(stringReader))));
        }
        finally
        {
            if ((stringReader != null))
            {
                stringReader.Dispose();
            }
        }
    }
    
    public static ItemProperties Deserialize(Stream s)
    {
        return ((ItemProperties)(SerializerXml.Deserialize(s)));
    }
    #endregion
    
    /// <summary>
    /// Serializes current ItemProperties object into file
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
    /// Deserializes xml markup from file into an ItemProperties object
    /// </summary>
    /// <param name="fileName">File to load and deserialize</param>
    /// <param name="obj">Output ItemProperties object</param>
    /// <param name="exception">output Exception value if deserialize failed</param>
    /// <returns>true if this Serializer can deserialize the object; otherwise, false</returns>
    public static bool LoadFromFile(string fileName, out ItemProperties obj, out Exception exception)
    {
        exception = null;
        obj = default(ItemProperties);
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
    
    public static bool LoadFromFile(string fileName, out ItemProperties obj)
    {
        Exception exception = null;
        return LoadFromFile(fileName, out obj, out exception);
    }
    
    public static ItemProperties LoadFromFile(string fileName)
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
