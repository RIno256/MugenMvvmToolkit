﻿#region Copyright

// ****************************************************************************
// <copyright file="ISerializableCallback.cs">
// Copyright (c) 2012-2015 Vyacheslav Volkov
// </copyright>
// ****************************************************************************
// <author>Vyacheslav Volkov</author>
// <email>vvs0205@outlook.com</email>
// <project>MugenMvvmToolkit</project>
// <web>https://github.com/MugenMvvmToolkit/MugenMvvmToolkit</web>
// <license>
// See license.txt in this solution or http://opensource.org/licenses/MS-PL
// </license>
// ****************************************************************************

#endregion

namespace MugenMvvmToolkit.Interfaces.Callbacks
{
    /// <summary>
    ///     Represents the serializable callback.
    /// </summary>
    public interface ISerializableCallback
    {
        /// <summary>
        ///     Invokes the callback using the specified operation result.
        /// </summary>
        object Invoke(IOperationResult result);
    }
}