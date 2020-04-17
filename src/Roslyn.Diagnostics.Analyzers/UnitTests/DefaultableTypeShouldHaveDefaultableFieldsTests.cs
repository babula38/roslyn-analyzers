﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using VerifyCS = Test.Utilities.CSharpCodeFixVerifier<
    Roslyn.Diagnostics.Analyzers.DefaultableTypeShouldHaveDefaultableFieldsAnalyzer,
    Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;

namespace Roslyn.Diagnostics.Analyzers.UnitTests
{
    public class DefaultableTypeShouldHaveDefaultableFieldsTests
    {
        private const string NonDefaultableAttribute = @"// <auto-generated/>
using System;

namespace Roslyn.Utilities {
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.GenericParameter)]
    sealed class NonDefaultableAttribute : Attribute { }
}
";

        public static IEnumerable<object[]> DefaultableTypes
        {
            get
            {
                yield return new object[] { "int" };
                yield return new object[] { "int?" };
                yield return new object[] { "DateTime" };
                yield return new object[] { "object?" };
                yield return new object[] { "IFormattable?" };
                yield return new object[] { "EventHandler?" };
                yield return new object[] { "StringComparison" };
            }
        }

        public static IEnumerable<object[]> DefaultableTypesNullableDisableContext
        {
            get
            {
                yield return new object[] { "int" };
                yield return new object[] { "int?" };
                yield return new object[] { "DateTime" };
                yield return new object[] { "object" };
                yield return new object[] { "IFormattable" };
                yield return new object[] { "EventHandler" };
                yield return new object[] { "StringComparison" };
            }
        }

        public static IEnumerable<object[]> NonDefaultableTypes
        {
            get
            {
                yield return new object[] { "object" };
                yield return new object[] { "IFormattable" };
                yield return new object[] { "EventHandler" };
                yield return new object[] { "NonDefaultableStruct" };
            }
        }

        [Theory]
        [MemberData(nameof(NonDefaultableTypes))]
        public async Task TestNonDefaultableStructWithNonDefaultableTypeField(string nonDefaultableType)
        {
            var code = $@"
#nullable enable

using System;
using Roslyn.Utilities;

[NonDefaultable]
struct NonDefaultableStruct {{ }}

[NonDefaultable]
struct NonDefaultableTestStruct {{
    {nonDefaultableType} field;
    {nonDefaultableType} Property {{ get; }}

    static {nonDefaultableType} staticField = default!;
    static {nonDefaultableType} StaticProperty {{ get; }} = default!;
}}
";

            await new VerifyCS.Test
            {
                LanguageVersion = LanguageVersion.CSharp8,
                TestState = { Sources = { code, NonDefaultableAttribute } },
            }.RunAsync();
        }

        [Theory]
        [MemberData(nameof(NonDefaultableTypes))]
        public async Task TestNonDefaultableStructWithNonDefaultableTypeNullableDisableField(string nonDefaultableType)
        {
            var code = $@"
#nullable disable

using System;
using Roslyn.Utilities;

[NonDefaultable]
struct NonDefaultableStruct {{ }}

[NonDefaultable]
struct NonDefaultableTestStruct {{
    {nonDefaultableType} field;
    {nonDefaultableType} Property {{ get; }}

    static {nonDefaultableType} staticField;
    static {nonDefaultableType} StaticProperty {{ get; }}
}}
";

            await new VerifyCS.Test
            {
                LanguageVersion = LanguageVersion.CSharp8,
                TestState = { Sources = { code, NonDefaultableAttribute } },
            }.RunAsync();
        }

        [Theory]
        [MemberData(nameof(DefaultableTypes))]
        public async Task TestNonDefaultableStructWithDefaultableTypeField(string defaultableType)
        {
            var code = $@"
#nullable enable

using System;
using Roslyn.Utilities;

[NonDefaultable]
struct NonDefaultableTestStruct {{
    {defaultableType} field;
    {defaultableType} Property {{ get; }}

    static {defaultableType} staticField;
    static {defaultableType} StaticProperty {{ get; }}
}}
";

            await new VerifyCS.Test
            {
                LanguageVersion = LanguageVersion.CSharp8,
                TestState = { Sources = { code, NonDefaultableAttribute } },
            }.RunAsync();
        }

        [Theory]
        [MemberData(nameof(DefaultableTypesNullableDisableContext))]
        public async Task TestNonDefaultableStructWithDefaultableTypeNullableDisableField(string defaultableType)
        {
            var code = $@"
#nullable disable

using System;
using Roslyn.Utilities;

[NonDefaultable]
struct NonDefaultableTestStruct {{
    {defaultableType} field;
    {defaultableType} Property {{ get; }}

    static {defaultableType} staticField;
    static {defaultableType} StaticProperty {{ get; }}
}}
";

            await new VerifyCS.Test
            {
                LanguageVersion = LanguageVersion.CSharp8,
                TestState = { Sources = { code, NonDefaultableAttribute } },
            }.RunAsync();
        }

        [Theory]
        [MemberData(nameof(DefaultableTypes))]
        public async Task TestDefaultableStructWithDefaultableTypeField(string defaultableType)
        {
            var code = $@"
#nullable enable

using System;

struct DefaultableStruct {{
    {defaultableType} field;
    {defaultableType} Property {{ get; }}

    static {defaultableType} staticField;
    static {defaultableType} StaticProperty {{ get; }}
}}
";

            await new VerifyCS.Test
            {
                LanguageVersion = LanguageVersion.CSharp8,
                TestState = { Sources = { code, NonDefaultableAttribute } },
            }.RunAsync();
        }

        [Theory]
        [MemberData(nameof(DefaultableTypes))]
        public async Task TestClassWithDefaultableTypeField(string defaultableType)
        {
            var code = $@"
#nullable enable

using System;

class TestClass {{
    {defaultableType} field;
    {defaultableType} Property {{ get; }}

    static {defaultableType} staticField;
    static {defaultableType} StaticProperty {{ get; }}
}}
";

            await new VerifyCS.Test
            {
                LanguageVersion = LanguageVersion.CSharp8,
                TestState = { Sources = { code, NonDefaultableAttribute } },
            }.RunAsync();
        }

        [Theory]
        [MemberData(nameof(DefaultableTypesNullableDisableContext))]
        public async Task TestClassWithDefaultableTypeNullableDisableField(string defaultableType)
        {
            var code = $@"
#nullable disable

using System;

class TestClass {{
    {defaultableType} field;
    {defaultableType} Property {{ get; }}

    static {defaultableType} staticField;
    static {defaultableType} StaticProperty {{ get; }}
}}
";

            await new VerifyCS.Test
            {
                LanguageVersion = LanguageVersion.CSharp8,
                TestState = { Sources = { code, NonDefaultableAttribute } },
            }.RunAsync();
        }

        [Theory]
        [MemberData(nameof(NonDefaultableTypes))]
        public async Task TestDefaultableStructWithNonDefaultableTypeField(string nonDefaultableType)
        {
            var code = $@"
#nullable enable

using System;
using Roslyn.Utilities;

[NonDefaultable]
struct NonDefaultableStruct {{ }}

struct DefaultableStruct {{
    {nonDefaultableType} [|field|];
    {nonDefaultableType} [|Property|] {{ get; }}

    static {nonDefaultableType} StaticField = default!;
    static {nonDefaultableType} StaticProperty {{ get; }} = default!;
}}
";

            await new VerifyCS.Test
            {
                LanguageVersion = LanguageVersion.CSharp8,
                TestState = { Sources = { code, NonDefaultableAttribute } },
            }.RunAsync();
        }

        [Theory]
        [MemberData(nameof(NonDefaultableTypes))]
        public async Task TestDefaultableStructWithNonDefaultableTypeNullableDisableField(string nonDefaultableType)
        {
            var code = $@"
#nullable disable

using System;
using Roslyn.Utilities;

[NonDefaultable]
struct NonDefaultableStruct {{ }}

struct DefaultableStruct {{
    {nonDefaultableType} field;
    {nonDefaultableType} Property {{ get; }}

    static {nonDefaultableType} StaticField;
    static {nonDefaultableType} StaticProperty {{ get; }}
}}
";

            await new VerifyCS.Test
            {
                LanguageVersion = LanguageVersion.CSharp8,
                TestState = { Sources = { code, NonDefaultableAttribute } },
            }.RunAsync();
        }
    }
}