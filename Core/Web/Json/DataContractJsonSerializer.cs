using System;
using System.Net;
using System.Collections.Generic;
using System.IO;

namespace Lin.Core.Web.Json
{
    internal class DataContractJsonSerializer
    {
        // Fields
        //private JavaScriptDeserializer jsonDeserializer;
        private JavaScriptSerializer jsonSerializer;
        //internal Dictionary<XmlQualifiedName, DataContract> knownDataContracts;
        //private ReadOnlyCollection<Type> knownTypeCollection;
        internal IList<Type> knownTypeList;
        //private DataContract rootContract;
        private Type rootType;

        // Methods
        public DataContractJsonSerializer(Type type)
            : this(type, null)
        {
        }

        public DataContractJsonSerializer(Type type, IEnumerable<Type> knownTypes)
        {
            this.Initialize(type, knownTypes);
        }

        //private void AddCollectionItemContractsToKnownDataContracts(DataContract traditionalDataContract)
        //{
        //    if (traditionalDataContract.KnownDataContracts != null)
        //    {
        //    Label_0100:
        //        foreach (KeyValuePair<XmlQualifiedName, DataContract> pair in traditionalDataContract.KnownDataContracts)
        //        {
        //            if (!object.ReferenceEquals(pair, null))
        //            {
        //                DataContract itemContract;
        //                for (CollectionDataContract contract = pair.Value as CollectionDataContract; contract != null; contract = itemContract as CollectionDataContract)
        //                {
        //                    itemContract = contract.ItemContract;
        //                    if (this.knownDataContracts == null)
        //                    {
        //                        this.knownDataContracts = new Dictionary<XmlQualifiedName, DataContract>();
        //                    }
        //                    if (!this.knownDataContracts.ContainsKey(itemContract.StableName))
        //                    {
        //                        this.knownDataContracts.Add(itemContract.StableName, itemContract);
        //                    }
        //                    if (contract.ItemType.IsGenericType && (contract.ItemType.GetGenericTypeDefinition() == typeof(KeyValue<,>)))
        //                    {
        //                        DataContract dataContract = DataContract.GetDataContract(Globals.TypeOfKeyValuePair.MakeGenericType(contract.ItemType.GetGenericArguments()));
        //                        if (!this.knownDataContracts.ContainsKey(dataContract.StableName))
        //                        {
        //                            this.knownDataContracts.Add(dataContract.StableName, dataContract);
        //                        }
        //                    }
        //                    if (!(itemContract is CollectionDataContract))
        //                    {
        //                        goto Label_0100;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //private void AddCollectionItemTypeToKnownTypes(Type knownType)
        //{
        //    Type type;
        //    for (Type type2 = knownType; CollectionDataContract.IsCollection(type2, out type); type2 = type)
        //    {
        //        if (type.IsGenericType && (type.GetGenericTypeDefinition() == Globals.TypeOfKeyValue))
        //        {
        //            type = Globals.TypeOfKeyValuePair.MakeGenericType(type.GetGenericArguments());
        //        }
        //        this.knownTypeList.Add(type);
        //    }
        //}

        //internal static void CheckIfTypeIsReference(DataContract dataContract)
        //{
        //    if (dataContract.IsReference)
        //    {
        //        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR2.GetString("JsonUnsupportedForIsReference", new object[] { DataContract.GetClrTypeFullName(dataContract.UnderlyingType), dataContract.IsReference })));
        //    }
        //}

        //internal object ConvertDataContractToObject(object value, DataContract contract, XmlObjectSerializerWriteContextComplexJson context, bool writeServerType, RuntimeTypeHandle declaredTypeHandle)
        //{
        //    if (context != null)
        //    {
        //        context.OnHandleReference(null, value, true);
        //    }
        //    try
        //    {
        //        if (contract is ObjectDataContract)
        //        {
        //            Type type = value.GetType();
        //            if (type != Globals.TypeOfObject)
        //            {
        //                return this.ConvertDataContractToObject(value, DataContract.GetDataContract(type), context, true, contract.UnderlyingType.TypeHandle);
        //            }
        //            return value;
        //        }
        //        if (contract is TimeSpanDataContract)
        //        {
        //            return XmlConvert.ToString((TimeSpan)value);
        //        }
        //        if (contract is QNameDataContract)
        //        {
        //            XmlQualifiedName name = (XmlQualifiedName)value;
        //            return (name.IsEmpty ? string.Empty : (name.Name + ":" + name.Namespace));
        //        }
        //        if (contract is PrimitiveDataContract)
        //        {
        //            return value;
        //        }
        //        if (contract is CollectionDataContract)
        //        {
        //            CollectionDataContract dataContract = contract as CollectionDataContract;
        //            switch (dataContract.Kind)
        //            {
        //                case CollectionKind.GenericDictionary:
        //                case CollectionKind.Dictionary:
        //                    return DataContractToObjectConverter.ConvertGenericDictionaryToArray(this, (IEnumerable)value, dataContract, context, writeServerType);
        //            }
        //            return DataContractToObjectConverter.ConvertGenericListToArray(this, (IEnumerable)value, dataContract, context, writeServerType);
        //        }
        //        if (contract is ClassDataContract)
        //        {
        //            ClassDataContract contract3 = contract as ClassDataContract;
        //            if (Globals.TypeOfScriptObject_IsAssignableFrom(contract3.UnderlyingType))
        //            {
        //                return this.ConvertScriptObjectToObject(value);
        //            }
        //            return DataContractToObjectConverter.ConvertClassDataContractToDictionary(this, (ClassDataContract)contract, value, context, writeServerType);
        //        }
        //        if (contract is EnumDataContract)
        //        {
        //            IConvertible convertible = value as IConvertible;
        //            if (((EnumDataContract)contract).IsULong)
        //            {
        //                return convertible.ToUInt64(null);
        //            }
        //            return convertible.ToInt64(null);
        //        }
        //        if (contract is XmlDataContract)
        //        {
        //            DataContractSerializer serializer = new DataContractSerializer(Type.GetTypeFromHandle(declaredTypeHandle), this.GetKnownTypesFromContext(context, (context == null) ? null : context.SerializerKnownTypeList));
        //            MemoryStream stream = new MemoryStream();
        //            serializer.WriteObject(stream, value);
        //            stream.Position = 0L;
        //            return new StreamReader(stream, Encoding.UTF8).ReadToEnd();
        //        }
        //    }
        //    finally
        //    {
        //        if (context != null)
        //        {
        //            context.OnEndHandleReference(null, value, true);
        //        }
        //    }
        //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException("UnknownDataContract: " + contract.Name));
        //}

        //internal object ConvertObjectToDataContract(DataContract contract, object value, XmlObjectSerializerReadContextComplexJson context)
        //{
        //    if (value != null)
        //    {
        //        if (contract is PrimitiveDataContract)
        //        {
        //            return this.ConvertObjectToPrimitiveDataContract(contract, value, context);
        //        }
        //        if (contract is CollectionDataContract)
        //        {
        //            return ObjectToDataContractConverter.ConvertICollectionToCollectionDataContract(this, (CollectionDataContract)contract, value, context);
        //        }
        //        if (contract is ClassDataContract)
        //        {
        //            ClassDataContract dataContract = contract as ClassDataContract;
        //            if (Globals.TypeOfScriptObject_IsAssignableFrom(dataContract.UnderlyingType))
        //            {
        //                return this.ConvertObjectToScriptObject(value);
        //            }
        //            return ObjectToDataContractConverter.ConvertDictionaryToClassDataContract(this, dataContract, (Dictionary<string, object>)value, context);
        //        }
        //        if (contract is EnumDataContract)
        //        {
        //            return Enum.ToObject(contract.UnderlyingType, ((EnumDataContract)contract).IsULong ? ulong.Parse(value.ToString(), NumberStyles.Float, NumberFormatInfo.InvariantInfo) : value);
        //        }
        //        if (contract is XmlDataContract)
        //        {
        //            DataContractSerializer serializer = new DataContractSerializer(contract.UnderlyingType, this.GetKnownTypesFromContext(context, (context == null) ? null : context.SerializerKnownTypeList));
        //            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes((string)value));
        //            return serializer.ReadObject(XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max));
        //        }
        //    }
        //    return value;
        //}

        //private object ConvertObjectToPrimitiveDataContract(DataContract contract, object value, XmlObjectSerializerReadContextComplexJson context)
        //{
        //    if (contract is TimeSpanDataContract)
        //    {
        //        return XmlConvert.ToTimeSpan(value.ToString());
        //    }
        //    if (contract is ByteArrayDataContract)
        //    {
        //        return ObjectToDataContractConverter.ConvertToArray(typeof(byte), (IList)value);
        //    }
        //    if (contract is GuidDataContract)
        //    {
        //        return new Guid(value.ToString());
        //    }
        //    if (contract is ObjectDataContract)
        //    {
        //        if (value is ICollection)
        //        {
        //            return this.ConvertObjectToDataContract(DataContract.GetDataContract(Globals.TypeOfObjectArray), value, context);
        //        }
        //        return this.TryParseJsonNumber(value);
        //    }
        //    if (contract is QNameDataContract)
        //    {
        //        return XmlObjectSerializerReadContextComplexJson.ParseQualifiedName(value.ToString());
        //    }
        //    if (contract is StringDataContract)
        //    {
        //        if (!(value is bool))
        //        {
        //            return value.ToString();
        //        }
        //        if (!((bool)value))
        //        {
        //            return "false";
        //        }
        //        return "true";
        //    }
        //    if (contract is UriDataContract)
        //    {
        //        return new Uri(value.ToString(), UriKind.RelativeOrAbsolute);
        //    }
        //    if (contract is DoubleDataContract)
        //    {
        //        if (value is float)
        //        {
        //            return (double)((float)value);
        //        }
        //        if (value is double)
        //        {
        //            return (double)value;
        //        }
        //        return double.Parse(value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture);
        //    }
        //    if (contract is DecimalDataContract)
        //    {
        //        return decimal.Parse(value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture);
        //    }
        //    return Convert.ChangeType(value, contract.UnderlyingType, CultureInfo.InvariantCulture);
        //}

        //private object ConvertObjectToScriptObject(object deserialzedValue)
        //{
        //    MemoryStream stream = new MemoryStream();
        //    new JavaScriptSerializer(stream).SerializeObject(deserialzedValue);
        //    stream.Flush();
        //    stream.Position = 0L;
        //    return Globals.ScriptObjectJsonDeserialize(new StreamReader(stream).ReadToEnd());
        //}

        //private object ConvertScriptObjectToObject(object value)
        //{
        //    string s = Globals.ScriptObjectJsonSerialize(value);
        //    using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(s)))
        //    {
        //        JavaScriptDeserializer deserializer = new JavaScriptDeserializer(stream);
        //        return deserializer.DeserializeObject();
        //    }
        //}

        //private static void DisallowMemberAccess(bool memberAccess)
        //{
        //    if (memberAccess)
        //    {
        //        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityException());
        //    }
        //}

        //internal static DataContract GetDataContract(DataContract declaredTypeContract, Type declaredType, Type objectType)
        //{
        //    DataContract dataContract = DataContractSerializer.GetDataContract(declaredTypeContract, declaredType, objectType);
        //    CheckIfTypeIsReference(dataContract);
        //    return dataContract;
        //}

        //private List<Type> GetKnownTypesFromContext(XmlObjectSerializerContext context, IList<Type> serializerKnownTypeList)
        //{
        //    List<Type> list = new List<Type>();
        //    if (context != null)
        //    {
        //        List<XmlQualifiedName> list2 = new List<XmlQualifiedName>();
        //        Dictionary<XmlQualifiedName, DataContract>[] dataContractDictionaries = context.scopedKnownTypes.dataContractDictionaries;
        //        if (dataContractDictionaries != null)
        //        {
        //            for (int i = 0; i < dataContractDictionaries.Length; i++)
        //            {
        //                Dictionary<XmlQualifiedName, DataContract> dictionary = dataContractDictionaries[i];
        //                if (dictionary != null)
        //                {
        //                    foreach (KeyValuePair<XmlQualifiedName, DataContract> pair in dictionary)
        //                    {
        //                        if (!list2.Contains(pair.Key))
        //                        {
        //                            list2.Add(pair.Key);
        //                            list.Add(pair.Value.UnderlyingType);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (serializerKnownTypeList != null)
        //        {
        //            list.AddRange(serializerKnownTypeList);
        //        }
        //    }
        //    return list;
        //}

        private void Initialize(Type type, IEnumerable<Type> knownTypes)
        {
            //XmlObjectSerializer.CheckNull(type, "type");
            this.rootType = type;
            if (knownTypes != null)
            {
                this.knownTypeList = new List<Type>();
                foreach (Type type2 in knownTypes)
                {
                    this.knownTypeList.Add(type2);
                    if (type2 != null)
                    {
                        //this.AddCollectionItemTypeToKnownTypes(type2);
                    }
                }
            }
        }

        //internal static void InvokeOnDeserialized(object value, DataContract contract, XmlObjectSerializerReadContextComplexJson context)
        //{
        //    if (contract is ClassDataContract)
        //    {
        //        ClassDataContract contract2 = contract as ClassDataContract;
        //        if (contract2.BaseContract != null)
        //        {
        //            InvokeOnDeserialized(value, contract2.BaseContract, context);
        //        }
        //        if (contract2.OnDeserialized != null)
        //        {
        //            bool memberAccess = contract2.RequiresMemberAccessForRead(null, JsonGlobals.JsonSerializationPatterns);
        //            try
        //            {
        //                DisallowMemberAccess(memberAccess);
        //                contract2.OnDeserialized.Invoke(value, new object[] { context.GetStreamingContext() });
        //            }
        //            catch (SecurityException exception)
        //            {
        //                if (!memberAccess)
        //                {
        //                    throw;
        //                }
        //                contract2.RequiresMemberAccessForRead(exception, JsonGlobals.JsonSerializationPatterns);
        //            }
        //            catch (TargetInvocationException exception2)
        //            {
        //                if (exception2.InnerException == null)
        //                {
        //                    throw;
        //                }
        //                throw exception2.InnerException;
        //            }
        //        }
        //    }
        //}

        //internal static void InvokeOnDeserializing(object value, DataContract contract, XmlObjectSerializerReadContextComplexJson context)
        //{
        //    if (contract is ClassDataContract)
        //    {
        //        ClassDataContract contract2 = contract as ClassDataContract;
        //        if (contract2.BaseContract != null)
        //        {
        //            InvokeOnDeserializing(value, contract2.BaseContract, context);
        //        }
        //        if (contract2.OnDeserializing != null)
        //        {
        //            bool memberAccess = contract2.RequiresMemberAccessForRead(null, JsonGlobals.JsonSerializationPatterns);
        //            try
        //            {
        //                DisallowMemberAccess(memberAccess);
        //                contract2.OnDeserializing.Invoke(value, new object[] { context.GetStreamingContext() });
        //            }
        //            catch (SecurityException exception)
        //            {
        //                if (!memberAccess)
        //                {
        //                    throw;
        //                }
        //                contract2.RequiresMemberAccessForRead(exception, JsonGlobals.JsonSerializationPatterns);
        //            }
        //            catch (TargetInvocationException exception2)
        //            {
        //                if (exception2.InnerException == null)
        //                {
        //                    throw;
        //                }
        //                throw exception2.InnerException;
        //            }
        //        }
        //    }
        //}

        //internal static void InvokeOnSerialized(object value, DataContract contract, XmlObjectSerializerWriteContextComplexJson context)
        //{
        //    if (contract is ClassDataContract)
        //    {
        //        ClassDataContract contract2 = contract as ClassDataContract;
        //        if (contract2.BaseContract != null)
        //        {
        //            InvokeOnSerialized(value, contract2.BaseContract, context);
        //        }
        //        if (contract2.OnSerialized != null)
        //        {
        //            bool memberAccess = contract2.RequiresMemberAccessForWrite(null, JsonGlobals.JsonSerializationPatterns);
        //            try
        //            {
        //                DisallowMemberAccess(memberAccess);
        //                contract2.OnSerialized.Invoke(value, new object[] { context.GetStreamingContext() });
        //            }
        //            catch (SecurityException exception)
        //            {
        //                if (!memberAccess)
        //                {
        //                    throw;
        //                }
        //                contract2.RequiresMemberAccessForWrite(exception, JsonGlobals.JsonSerializationPatterns);
        //            }
        //            catch (TargetInvocationException exception2)
        //            {
        //                if (exception2.InnerException == null)
        //                {
        //                    throw;
        //                }
        //                throw exception2.InnerException;
        //            }
        //        }
        //    }
        //}

        //internal static void InvokeOnSerializing(object value, DataContract contract, XmlObjectSerializerWriteContextComplexJson context)
        //{
        //    if (contract is ClassDataContract)
        //    {
        //        ClassDataContract contract2 = contract as ClassDataContract;
        //        if (contract2.BaseContract != null)
        //        {
        //            InvokeOnSerializing(value, contract2.BaseContract, context);
        //        }
        //        if (contract2.OnSerializing != null)
        //        {
        //            bool memberAccess = contract2.RequiresMemberAccessForWrite(null, JsonGlobals.JsonSerializationPatterns);
        //            try
        //            {
        //                DisallowMemberAccess(memberAccess);
        //                contract2.OnSerializing.Invoke(value, new object[] { context.GetStreamingContext() });
        //            }
        //            catch (SecurityException exception)
        //            {
        //                if (!memberAccess)
        //                {
        //                    throw;
        //                }
        //                contract2.RequiresMemberAccessForWrite(exception, JsonGlobals.JsonSerializationPatterns);
        //            }
        //            catch (TargetInvocationException exception2)
        //            {
        //                if (exception2.InnerException == null)
        //                {
        //                    throw;
        //                }
        //                throw exception2.InnerException;
        //            }
        //        }
        //    }
        //}

        public object ReadObject(Stream stream)
        {
            object obj3 = null;
            //try
            //{
            //    DataContract rootContract = this.RootContract;
            //    this.AddCollectionItemContractsToKnownDataContracts(rootContract);
            JavaScriptDeserializer jsonDeserializer = new JavaScriptDeserializer(stream);
            obj3 = jsonDeserializer.DeserializeObject();
            //    XmlObjectSerializerReadContextComplexJson context = new XmlObjectSerializerReadContextComplexJson(this, this.RootContract);
            //    obj3 = this.ConvertObjectToDataContract(this.RootContract, this.jsonDeserializer.DeserializeObject(), context);
            //}
            //catch (Exception exception)
            //{
            //    if (((exception is TargetException) || (exception is TargetInvocationException)) || ((exception is FormatException) || (exception is OverflowException)))
            //    {
            //        throw XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.GetTypeInfoError("ErrorDeserializing", this.rootType, exception), exception);
            //    }
            //    throw;
            //}
            return obj3;
        }

        private readonly static char[] FloatingPointCharacters = new char[] { '.', 'e', 'E' };
        private object TryParseJsonNumber(object value)
        {
            string input = value as string;
            if ((input != null) && (input.IndexOfAny(FloatingPointCharacters) >= 0))
            {
                return JavaScriptObjectDeserializer.ParseJsonNumberAsDoubleOrDecimal(input);
            }
            return value;
        }

        public void WriteObject(Stream stream, object graph)
        {
            this.jsonSerializer = new JavaScriptSerializer(stream);
            //DataContract rootContract = this.RootContract;
            //Type underlyingType = rootContract.UnderlyingType;
            //Type objectType = (graph == null) ? underlyingType : graph.GetType();
            //XmlWriterDelegator xmlWriter = null;
            if (graph == null)
            {
                this.jsonSerializer.SerializeObject(null);
            }
            else
            {
                this.jsonSerializer.SerializeObject(graph);
            }
            stream.Flush();
            //stream.Close();
            //else if (underlyingType == objectType)
            //{
            //    if (rootContract.CanContainReferences)
            //    {
            //        XmlObjectSerializerWriteContextComplexJson.CreateContext(this, rootContract).SerializeWithoutXsiType(rootContract, xmlWriter, graph, underlyingType.TypeHandle);
            //    }
            //    else
            //    {
            //        this.WriteObjectInternal(graph, rootContract, null, false, underlyingType.TypeHandle);
            //    }
            //}
            //else
            //{
            //    XmlObjectSerializerWriteContextComplexJson json2 = XmlObjectSerializerWriteContextComplexJson.CreateContext(this, this.RootContract);
            //    rootContract = DataContractSerializer.GetDataContract(rootContract, underlyingType, objectType);
            //    if (rootContract.CanContainReferences)
            //    {
            //        json2.SerializeWithXsiTypeAtTopLevel(rootContract, xmlWriter, graph, underlyingType.TypeHandle);
            //    }
            //    else
            //    {
            //        json2.SerializeWithoutXsiType(rootContract, xmlWriter, graph, underlyingType.TypeHandle);
            //    }
            //}
        }

        //internal void WriteObjectInternal(object value, DataContract contract, XmlObjectSerializerWriteContextComplexJson context, bool writeServerType, RuntimeTypeHandle declaredTypeHandle)
        //{
        //    this.jsonSerializer.SerializeObject(this.ConvertDataContractToObject(value, contract, context, writeServerType, declaredTypeHandle));
        //}

        //// Properties
        //internal Dictionary<XmlQualifiedName, DataContract> KnownDataContracts
        //{
        //    get
        //    {
        //        if ((this.knownDataContracts == null) && (this.knownTypeList != null))
        //        {
        //            this.knownDataContracts = XmlObjectSerializerContext.GetDataContractsForKnownTypes(this.knownTypeList);
        //        }
        //        return this.knownDataContracts;
        //    }
        //}

        //public ReadOnlyCollection<Type> KnownTypes
        //{
        //    get
        //    {
        //        if (this.knownTypeCollection == null)
        //        {
        //            if (this.knownTypeList != null)
        //            {
        //                this.knownTypeCollection = new ReadOnlyCollection<Type>(this.knownTypeList);
        //            }
        //            else
        //            {
        //                this.knownTypeCollection = new ReadOnlyCollection<Type>(Globals.EmptyTypeArray);
        //            }
        //        }
        //        return this.knownTypeCollection;
        //    }
        //}

        //private DataContract RootContract
        //{
        //    get
        //    {
        //        if (this.rootContract == null)
        //        {
        //            this.rootContract = DataContract.GetDataContract(this.rootType);
        //            CheckIfTypeIsReference(this.rootContract);
        //        }
        //        return this.rootContract;
        //    }
        //}
    }
}
