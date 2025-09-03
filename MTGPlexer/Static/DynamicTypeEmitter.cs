namespace MTGPlexer.Static;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

/// <summary>
/// A static class responsible for emitting new C# types at runtime based on a template type.
/// The emitted types will have "_Many" appended to their names and to specified properties,
/// with corresponding changes to property types and constructor logic.
/// </summary>
public static class DynamicTypeEmitter
{
    private static readonly AssemblyBuilder _asmBuilder =
        AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicTokenUnits"), AssemblyBuilderAccess.Run);

    private static readonly ModuleBuilder _moduleBuilder =
        _asmBuilder.DefineDynamicModule("MainModule");

    // Static dictionaries to map IL byte values to OpCode instances for efficient parsing.
    private static readonly Dictionary<short, OpCode> _twoByteOpCodes = typeof(OpCodes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Select(f => (OpCode)f.GetValue(null))
        .Where(op => op.Size == 2)
        .ToDictionary(op => op.Value);

    private static readonly Dictionary<short, OpCode> _oneByteOpCodes = typeof(OpCodes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Select(f => (OpCode)f.GetValue(null))
        .Where(op => op.Size == 1)
        .ToDictionary(op => op.Value);

    /// <summary>
    /// Scans a type for properties decorated with [OptionalMany] and, if found, emits a new dynamic type.
    /// If no properties are marked with [OptionalMany] (either directly or on their type), this method returns null.
    /// </summary>
    /// <param name="originalType">The source type to be duplicated and modified.</param>
    /// <returns>The newly created dynamic Type, or null if no [OptionalMany] properties were found.</returns>
    public static Type EmitManyType(Type originalType)
    {
        // Discover properties that have the [OptionalMany] attribute on the property itself
        // or on the property's type definition.
        var manyProps = originalType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(prop => prop.IsDefined(typeof(OptionalManyAttribute)) || prop.PropertyType.IsDefined(typeof(OptionalManyAttribute)))
            .ToList();

        // If no such properties exist, there is nothing to do.
        if (!manyProps.Any())
        {
            return null;
        }

        return EmitManyTypeInternal(originalType, manyProps);
    }

    /// <summary>
    /// Internal implementation that emits a new dynamic type based on an original type and a pre-defined
    /// list of properties that require transformation.
    /// </summary>
    private static Type EmitManyTypeInternal(Type originalType, IEnumerable<PropertyInfo> manyProps)
    {
        var manyPropNames = new HashSet<string>(manyProps.Select(p => p.Name));
        string newTypeName = $"{originalType.Name}_Many";

        // 1. Define the new type with "_Many" suffix and the same base type.
        TypeBuilder typeBuilder = _moduleBuilder.DefineType(
            newTypeName,
            TypeAttributes.Public | TypeAttributes.Class,
            originalType.BaseType);

        // 2. Replicate properties, modifying the ones marked as "many".
        foreach (var prop in originalType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            if (manyPropNames.Contains(prop.Name))
            {
                // This is an OptionalMany property.
                // 3. Append "_Many" to the property name.
                string newPropName = $"{prop.Name}_Many";
                // 4. Substitute the type T with ManyToken<T>.
                Type newPropType = typeof(ManyToken<>).MakeGenericType(prop.PropertyType);
                CreateProperty(typeBuilder, newPropName, newPropType);
            }
            else
            {
                // This is a regular property, so we replicate it as is.
                CreateProperty(typeBuilder, prop.Name, prop.PropertyType);
            }
        }

        // 5. Replicate constructors, modifying nameof() references.
        ReplicateConstructors(typeBuilder, originalType, manyPropNames);

        return typeBuilder.CreateType();
    }


    /// <summary>
    /// Defines a property with a private backing field and default get/set methods.
    /// </summary>
    private static void CreateProperty(TypeBuilder typeBuilder, string name, Type type)
    {
        FieldBuilder fieldBuilder = typeBuilder.DefineField($"_{name.ToLower()}", type, FieldAttributes.Private);
        PropertyBuilder propBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.HasDefault, type, null);
        const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        MethodBuilder getMethod = typeBuilder.DefineMethod($"get_{name}", getSetAttr, type, Type.EmptyTypes);
        ILGenerator getIL = getMethod.GetILGenerator();
        getIL.Emit(OpCodes.Ldarg_0);
        getIL.Emit(OpCodes.Ldfld, fieldBuilder);
        getIL.Emit(OpCodes.Ret);
        propBuilder.SetGetMethod(getMethod);

        MethodBuilder setMethod = typeBuilder.DefineMethod($"set_{name}", getSetAttr, null, new[] { type });
        ILGenerator setIL = setMethod.GetILGenerator();
        setIL.Emit(OpCodes.Ldarg_0);
        setIL.Emit(OpCodes.Ldarg_1);
        setIL.Emit(OpCodes.Stfld, fieldBuilder);
        setIL.Emit(OpCodes.Ret);
        propBuilder.SetSetMethod(setMethod);
    }

    /// <summary>
    /// Iterates over the original type's constructors and replicates them in the new type.
    /// </summary>
    private static void ReplicateConstructors(TypeBuilder typeBuilder, Type originalType, HashSet<string> manyPropNames)
    {
        foreach (var ctor in originalType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            ReplicateConstructorWithModifiedNameof(typeBuilder, ctor, manyPropNames);
        }
    }

    /// <summary>
    /// Creates a new constructor by replicating the IL of an existing constructor,
    /// while modifying string literals that correspond to "many" property names.
    /// </summary>
    private static void ReplicateConstructorWithModifiedNameof(TypeBuilder typeBuilder, ConstructorInfo originalCtor, HashSet<string> manyPropNames)
    {
        var parameters = originalCtor.GetParameters();
        var paramTypes = parameters.Select(p => p.ParameterType).ToArray();

        ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(originalCtor.Attributes, originalCtor.CallingConvention, paramTypes);
        ILGenerator il = ctorBuilder.GetILGenerator();

        var methodBody = originalCtor.GetMethodBody();
        byte[] ilBytes = methodBody?.GetILAsByteArray();
        if (ilBytes == null) return;

        var reader = new ILReader(originalCtor.Module, ilBytes);

        // Replicate each IL instruction, transforming where necessary.
        while (reader.HasNext)
        {
            var instruction = reader.Read();

            // If we find a "load string" (ldstr) instruction, check if the string needs to be modified.
            if (instruction.OpCode == OpCodes.Ldstr)
            {
                var literal = instruction.Operand as string;
                if (literal != null && manyPropNames.Contains(literal))
                {
                    // This string matches a property we're renaming.
                    // Emit a new string with the "_Many" suffix.
                    il.Emit(OpCodes.Ldstr, $"{literal}_Many");
                }
                else
                {
                    // It's a regular string, so emit it as is.
                    instruction.Emit(il);
                }
            }
            else
            {
                // For all other instructions, replicate them directly.
                instruction.Emit(il);
            }
        }
    }

    #region ILReader Helper Classes

    /// <summary>
    /// A helper class to read IL instructions from a byte array.
    /// This is a simplified IL disassembler focused on the needs of this emitter.
    /// </summary>
    private class ILReader
    {
        private readonly Module _module;
        private readonly byte[] _ilBytes;
        private int _position;

        public ILReader(Module module, byte[] ilBytes)
        {
            _module = module;
            _ilBytes = ilBytes;
            _position = 0;
        }

        public bool HasNext => _position < _ilBytes.Length;

        public ILInstruction Read()
        {
            int opCodeValue = _ilBytes[_position++];

            if (opCodeValue == 0xFE) // Two-byte opcode prefix.
            {
                opCodeValue = (short)(opCodeValue << 8 | _ilBytes[_position++]);
                var opCode = _twoByteOpCodes[(short)opCodeValue];
                return new ILInstruction(opCode, ReadOperand(opCode));
            }
            else
            {
                var opCode = _oneByteOpCodes[(short)opCodeValue];
                return new ILInstruction(opCode, ReadOperand(opCode));
            }
        }

        private object ReadOperand(OpCode opCode)
        {
            switch (opCode.OperandType)
            {
                case OperandType.InlineString:
                    return _module.ResolveString(ReadInt32());
                case OperandType.InlineMethod:
                    return _module.ResolveMethod(ReadInt32());
                case OperandType.InlineField:
                    return _module.ResolveField(ReadInt32());
                case OperandType.InlineType:
                    return _module.ResolveType(ReadInt32());
                case OperandType.ShortInlineI:
                    return (sbyte)_ilBytes[_position++];
                case OperandType.InlineI:
                    return ReadInt32();
                case OperandType.ShortInlineBrTarget:
                    return (sbyte)_ilBytes[_position++];
                case OperandType.InlineBrTarget:
                    return ReadInt32();
                case OperandType.InlineNone:
                    return null;
                default:
                    throw new NotSupportedException($"Unsupported operand type: {opCode.OperandType}");
            }
        }

        private int ReadInt32()
        {
            int value = BitConverter.ToInt32(_ilBytes, _position);
            _position += 4;
            return value;
        }
    }

    /// <summary>
    /// Represents a single IL instruction (OpCode and its optional Operand).
    /// </summary>
    private class ILInstruction
    {
        public OpCode OpCode { get; }
        public object Operand { get; }

        public ILInstruction(OpCode opCode, object operand)
        {
            OpCode = opCode;
            Operand = operand;
        }

        public void Emit(ILGenerator il)
        {
            if (Operand == null)
            {
                il.Emit(OpCode);
                return;
            }

            // Call the correct ILGenerator.Emit overload based on the operand's type.
            switch (Operand)
            {
                case string s:
                    il.Emit(OpCode, s);
                    break;
                case ConstructorInfo ci:
                    il.Emit(OpCode, ci);
                    break;
                case MethodInfo mi:
                    il.Emit(OpCode, mi);
                    break;
                case FieldInfo fi:
                    il.Emit(OpCode, fi);
                    break;
                case Type t:
                    il.Emit(OpCode, t);
                    break;
                case int i:
                    il.Emit(OpCode, i);
                    break;
                case sbyte sb:
                    il.Emit(OpCode, sb);
                    break;
                default:
                    throw new NotSupportedException($"Emit for operand of type {Operand.GetType()} is not supported.");
            }
        }
    }

    #endregion
}