// Copyright 2016-2017 Confluent Inc., 2015-2016 Andreas Heider
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Derived from: rdkafka-dotnet, licensed under the 2-clause BSD License.
//
// Refer to LICENSE for more information.

using System.Collections.Generic;

namespace iWaterDataCollector.Kafka.Internal.Extensions
{
    /// <summary>
    ///     Extension methods for the <see cref="IDictionary{TKey,TValue}"/> class.
    /// </summary>
    internal static class DictionaryExtensions
    {
        internal static string[] ToStringArray(this IDictionary<string, string> dictionary)
        {
            if (dictionary == null) return null;
            if (dictionary.Count == 0) return new string[0];

            var result = new string[dictionary.Count * 2];
            var index = 0;
            foreach (var pair in dictionary)
            {
                result[index++] = pair.Key;
                result[index++] = pair.Value;
            }

            return result;
        }
    }
}
