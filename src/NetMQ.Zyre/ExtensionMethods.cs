/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
using System;

namespace NetMQ.Zyre
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get the first 6 characters of the ToString() of the guid.
        /// </summary>
        /// <param name="guid">the GUID</param>
        /// <returns>the first 6 characters of the ToString()</returns>
        public static string ToShortString6(this Guid guid)
        {
            var str = guid.ToString();
            return str.Substring(0, 6);
        }
    }
}
