//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Messages
{
    [Serializable]
    public struct MessageId : IIsId, IEquatable<MessageId>
    {
        private const MessageId s_NoneId = new MessageId(Guid.NewGuid()); 

        public static MessageId None
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private readonly Guid m_Id;

        private MessageId(Guid id)
        {
            m_Id = id;
        }

        public MessageId(MessageId idToCopy)
        { }

        public MessageId(IIsId idToCopy)
        { }

        public MessageId Copy()
        { }

        public bool Equals(MessageId other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}