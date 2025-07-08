// Copyright 2016-2017 Confluent Inc.
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
// Refer to LICENSE for more information.

using System;
using System.IO;

namespace iWaterDataCollector.Kafka.Impl
{
    /// <summary>
    /// Based on Internal.Microsoft.DotNet.PlatformAbstractions.Native.PlatformApis
    /// https://github.com/xunit/visualstudio.xunit/blob/613251df49602256c49001501c71e1f42bc82df9/src/xunit.runner.visualstudio/Utility/AssemblyResolution/Microsoft.DotNet.PlatformAbstractions/Native/PlatformApis.cs
    /// Code is not consumed directly because this would require adding dependency for Microsoft.DotNet.PlatformAbstractions.
    /// Support for this package was removed in .Net 5:
    /// https://docs.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/5.0/platformabstractions-package-removed
    /// </summary>
    class PlatformApis
    {
        private static readonly Lazy<PlatformApis.DistroInfo> _distroInfo = new Lazy<PlatformApis.DistroInfo>(new Func<PlatformApis.DistroInfo>(PlatformApis.LoadDistroInfo));

        public static string GetOSName()
        {
            return PlatformApis.GetDistroId() ?? "Linux";
        }

        public static string GetOSVersion()
        {
            return PlatformApis.GetDistroVersionId() ?? string.Empty;
        }

        private static string GetDistroId() => PlatformApis._distroInfo.Value?.Id;
        private static string GetDistroVersionId() => PlatformApis._distroInfo.Value?.VersionId;

        private static DistroInfo LoadDistroInfo()
        {
            DistroInfo distroInfo = (DistroInfo)null;
            if (File.Exists("/etc/os-release"))
            {
                string[] strArray = File.ReadAllLines("/etc/os-release");
                distroInfo = new DistroInfo();
                foreach (string str in strArray)
                {
                    if (str.StartsWith("ID=", StringComparison.Ordinal))
                        distroInfo.Id = str.Substring(3).Trim('"', '\'');
                    else if (str.StartsWith("VERSION_ID=", StringComparison.Ordinal))
                        distroInfo.VersionId = str.Substring(11).Trim('"', '\'');
                }
            }
            else if (File.Exists("/etc/redhat-release"))
            {
                string[] strArray = File.ReadAllLines("/etc/redhat-release");
                if (strArray.Length >= 1)
                {
                    string str = strArray[0];
                    if (str.StartsWith("Red Hat Enterprise Linux Server release 6.") || str.StartsWith("CentOS release 6."))
                    {
                        distroInfo = new DistroInfo();
                        distroInfo.Id = "rhel";
                        distroInfo.VersionId = "6";
                    }
                }
            }
            if (distroInfo != null)
                distroInfo = NormalizeDistroInfo(distroInfo);
            return distroInfo;
        }

        private static DistroInfo NormalizeDistroInfo(
          DistroInfo distroInfo)
        {
            string versionId = distroInfo.VersionId;
            int length = versionId != null ? versionId.IndexOf('.') : -1;
            if (length != -1 && distroInfo.Id == "alpine")
                length = distroInfo.VersionId.IndexOf('.', length + 1);
            if (length != -1 && (distroInfo.Id == "rhel" || distroInfo.Id == "alpine"))
                distroInfo.VersionId = distroInfo.VersionId.Substring(0, length);
            return distroInfo;
        }

        class DistroInfo
        {
            public string Id;
            public string VersionId;
        }
    }
}
