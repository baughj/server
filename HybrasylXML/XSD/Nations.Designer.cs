// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by Xsd2Code++. Version 4.2.0.72
//    <NameSpace>Hybrasyl.Nations</NameSpace><Collection>List</Collection><codeType>CSharp</codeType><EnableDataBinding>False</EnableDataBinding><GenerateCloneMethod>False</GenerateCloneMethod><GenerateDataContracts>False</GenerateDataContracts><DataMemberNameArg>OnlyIfDifferent</DataMemberNameArg><DataMemberOnXmlIgnore>False</DataMemberOnXmlIgnore><CodeBaseTag>Net20</CodeBaseTag><InitializeFields>AllExceptOptional</InitializeFields><GenerateUnusedComplexTypes>False</GenerateUnusedComplexTypes><GenerateUnusedSimpleTypes>False</GenerateUnusedSimpleTypes><GenerateXMLAttributes>True</GenerateXMLAttributes><OrderXMLAttrib>False</OrderXMLAttrib><EnableLazyLoading>False</EnableLazyLoading><VirtualProp>False</VirtualProp><PascalCase>False</PascalCase><AutomaticProperties>False</AutomaticProperties><PropNameSpecified>None</PropNameSpecified><PrivateFieldName>StartWithUnderscore</PrivateFieldName><PrivateFieldNamePrefix></PrivateFieldNamePrefix><EnableRestriction>False</EnableRestriction><RestrictionMaxLenght>False</RestrictionMaxLenght><RestrictionRegEx>False</RestrictionRegEx><RestrictionRange>False</RestrictionRange><ValidateProperty>False</ValidateProperty><ClassNamePrefix></ClassNamePrefix><ClassLevel>Public</ClassLevel><PartialClass>True</PartialClass><ClassesInSeparateFiles>False</ClassesInSeparateFiles><ClassesInSeparateFilesDir></ClassesInSeparateFilesDir><TrackingChangesEnable>False</TrackingChangesEnable><GenTrackingClasses>False</GenTrackingClasses><HidePrivateFieldInIDE>False</HidePrivateFieldInIDE><EnableSummaryComment>False</EnableSummaryComment><EnableAppInfoSettings>False</EnableAppInfoSettings><EnableExternalSchemasCache>False</EnableExternalSchemasCache><EnableDebug>False</EnableDebug><EnableWarn>False</EnableWarn><ExcludeImportedTypes>False</ExcludeImportedTypes><ExpandNesteadAttributeGroup>False</ExpandNesteadAttributeGroup><CleanupCode>False</CleanupCode><EnableXmlSerialization>False</EnableXmlSerialization><SerializeMethodName>Serialize</SerializeMethodName><DeserializeMethodName>Deserialize</DeserializeMethodName><SaveToFileMethodName>SaveToFile</SaveToFileMethodName><LoadFromFileMethodName>LoadFromFile</LoadFromFileMethodName><EnableEncoding>False</EnableEncoding><EnableXMLIndent>False</EnableXMLIndent><IndentChar>Indent1Space</IndentChar><NewLineAttr>False</NewLineAttr><OmitXML>False</OmitXML><Encoder>UTF8</Encoder><Serializer>XmlSerializer</Serializer><sspNullable>False</sspNullable><sspString>False</sspString><sspCollection>False</sspCollection><sspComplexType>False</sspComplexType><sspSimpleType>False</sspSimpleType><sspEnumType>False</sspEnumType><XmlSerializerEvent>False</XmlSerializerEvent><BaseClassName>EntityBase</BaseClassName><UseBaseClass>False</UseBaseClass><GenBaseClass>False</GenBaseClass><CustomUsings></CustomUsings><AttributesToExlude></AttributesToExlude>
//  </auto-generated>
// ------------------------------------------------------------------------------
#pragma warning disable
namespace Hybrasyl.Nations
{
    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Collections;
    using System.Xml.Schema;
    using System.ComponentModel;
    using System.Xml;
    using System.Collections.Generic;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2046.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.hybrasyl.com/XML/Nations")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.hybrasyl.com/XML/Nations", IsNullable=false)]
    public partial class Nation
    {
        
        #region Private fields
        private string _name;
        
        private string _description;
        
        private List<SpawnPoint> _spawnPoints;
        
        private byte _flag;
        
        private bool _default;
        #endregion
        
        public Nation()
        {
            this._default = false;
        }
        
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }
        
        public string Description
        {
            get
            {
                return this._description;
            }
            set
            {
                this._description = value;
            }
        }
        
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public List<SpawnPoint> SpawnPoints
        {
            get
            {
                return this._spawnPoints;
            }
            set
            {
                this._spawnPoints = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Flag
        {
            get
            {
                return this._flag;
            }
            set
            {
                this._flag = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool Default
        {
            get
            {
                return this._default;
            }
            set
            {
                this._default = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2046.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.hybrasyl.com/XML/Nations")]
    public partial class SpawnPoint
    {
        
        #region Private fields
        private string _mapName;
        
        private byte _x;
        
        private byte _y;
        #endregion
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MapName
        {
            get
            {
                return this._mapName;
            }
            set
            {
                this._mapName = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte X
        {
            get
            {
                return this._x;
            }
            set
            {
                this._x = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Y
        {
            get
            {
                return this._y;
            }
            set
            {
                this._y = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2046.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.hybrasyl.com/XML/Nations")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.hybrasyl.com/XML/Nations", IsNullable=false)]
    public partial class Territory
    {
        
        #region Private fields
        private List<object> _map;
        
        private string _nation;
        #endregion
        
        public Territory()
        {
            this._map = new List<object>();
        }
        
        [System.Xml.Serialization.XmlElementAttribute("Map")]
        public List<object> Map
        {
            get
            {
                return this._map;
            }
            set
            {
                this._map = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Nation
        {
            get
            {
                return this._nation;
            }
            set
            {
                this._nation = value;
            }
        }
    }
}
#pragma warning restore