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
[XmlRootAttribute(Namespace="http://www.hybrasyl.com/XML/Hybrasyl/2020-02", IsNullable=false)]
public partial class Recipe
{
    #region Private fields
    private string _name;
    private RecipeItem _item;
    private RecipeDuration _duration;
    private string _description;
    private RecipeIngredients _ingredients;
    private static XmlSerializer _serializerXml;
    #endregion
    
    public Recipe()
    {
        _ingredients = new RecipeIngredients();
        _duration = new RecipeDuration();
        _item = new RecipeItem();
    }
    
    [StringLengthAttribute(255, MinimumLength=1)]
    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;
        }
    }
    
    public RecipeItem Item
    {
        get
        {
            return _item;
        }
        set
        {
            _item = value;
        }
    }
    
    public RecipeDuration Duration
    {
        get
        {
            return _duration;
        }
        set
        {
            _duration = value;
        }
    }
    
    [StringLengthAttribute(255, MinimumLength=1)]
    public string Description
    {
        get
        {
            return _description;
        }
        set
        {
            _description = value;
        }
    }
    
    public RecipeIngredients Ingredients
    {
        get
        {
            return _ingredients;
        }
        set
        {
            _ingredients = value;
        }
    }
    
    private static XmlSerializer SerializerXml
    {
        get
        {
            if ((_serializerXml == null))
            {
                _serializerXml = new XmlSerializerFactory().CreateSerializer(typeof(Recipe));
            }
            return _serializerXml;
        }
    }
    
    #region Serialize/Deserialize
    /// <summary>
    /// Serialize Recipe object
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
    /// Deserializes Recipe object
    /// </summary>
    /// <param name="input">string to deserialize</param>
    /// <param name="obj">Output Recipe object</param>
    /// <param name="exception">output Exception value if deserialize failed</param>
    /// <returns>true if this Serializer can deserialize the object; otherwise, false</returns>
    public static bool Deserialize(string input, out Recipe obj, out Exception exception)
    {
        exception = null;
        obj = default(Recipe);
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
    
    public static bool Deserialize(string input, out Recipe obj)
    {
        Exception exception = null;
        return Deserialize(input, out obj, out exception);
    }
    
    public static Recipe Deserialize(string input)
    {
        StringReader stringReader = null;
        try
        {
            stringReader = new StringReader(input);
            return ((Recipe)(SerializerXml.Deserialize(XmlReader.Create(stringReader))));
        }
        finally
        {
            if ((stringReader != null))
            {
                stringReader.Dispose();
            }
        }
    }
    
    public static Recipe Deserialize(Stream s)
    {
        return ((Recipe)(SerializerXml.Deserialize(s)));
    }
    #endregion
    
    /// <summary>
    /// Serializes current Recipe object into file
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
    /// Deserializes xml markup from file into an Recipe object
    /// </summary>
    /// <param name="fileName">File to load and deserialize</param>
    /// <param name="obj">Output Recipe object</param>
    /// <param name="exception">output Exception value if deserialize failed</param>
    /// <returns>true if this Serializer can deserialize the object; otherwise, false</returns>
    public static bool LoadFromFile(string fileName, out Recipe obj, out Exception exception)
    {
        exception = null;
        obj = default(Recipe);
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
    
    public static bool LoadFromFile(string fileName, out Recipe obj)
    {
        Exception exception = null;
        return LoadFromFile(fileName, out obj, out exception);
    }
    
    public static Recipe LoadFromFile(string fileName)
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
