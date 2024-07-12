// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// Copyright 2020 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;

namespace Serilog.Sinks.BrowserConsole;

sealed class ElementReferenceJsonConverter : JsonConverter<ElementReference>
{
    static readonly JsonEncodedText IdProperty = JsonEncodedText.Encode("__internalId");

    readonly ElementReferenceContext _elementReferenceContext;

    public ElementReferenceJsonConverter(ElementReferenceContext elementReferenceContext)
    {
        _elementReferenceContext = elementReferenceContext;
    }

    public override ElementReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? id = null;
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                if (reader.ValueTextEquals(IdProperty.EncodedUtf8Bytes))
                {
                    reader.Read();
                    id = reader.GetString();
                }
                else
                {
                    throw new JsonException($"Unexpected JSON property '{reader.GetString()}'.");
                }
            }
            else
            {
                throw new JsonException($"Unexpected JSON Token {reader.TokenType}.");
            }
        }

        if (id is null)
        {
            throw new JsonException("__internalId is required.");
        }

        return new ElementReference(id, _elementReferenceContext);
    }

    public override void Write(Utf8JsonWriter writer, ElementReference value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString(IdProperty, value.Id);
        writer.WriteEndObject();
    }
}