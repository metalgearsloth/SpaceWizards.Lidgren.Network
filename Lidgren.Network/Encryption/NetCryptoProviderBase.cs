﻿using System;
using System.IO;
using System.Security.Cryptography;

namespace Lidgren.Network
{
	public abstract class NetCryptoProviderBase : NetEncryption
	{
		protected SymmetricAlgorithm m_algorithm;

		public NetCryptoProviderBase(NetPeer peer, SymmetricAlgorithm algo)
			: base(peer)
		{
			m_algorithm = algo;
			m_algorithm.GenerateKey();
			m_algorithm.GenerateIV();
		}

		public override void SetKey(ReadOnlySpan<byte> data)
		{
			int len = m_algorithm.Key.Length;
			var key = new byte[len];
			for (int i = 0; i < len; i++)
				key[i] = data[i % data.Length];
			m_algorithm.Key = key;

			len = m_algorithm.IV.Length;
			key = new byte[len];
			for (int i = 0; i < len; i++)
				key[len - 1 - i] = data[i % data.Length];
			m_algorithm.IV = key;
		}

		public override bool Encrypt(NetOutgoingMessage msg)
		{
			int unEncLenBits = msg.LengthBits;

			var ms = new MemoryStream();
			var cs = new CryptoStream(ms, m_algorithm.CreateEncryptor(), CryptoStreamMode.Write);
			cs.Write(msg.m_data, 0, msg.LengthBytes);
			cs.FlushFinalBlock();

			var buffer = ms.GetBuffer();

			var newLength = ((int)ms.Length + 4) * 8;
			msg.EnsureBufferSize(newLength);
			msg.LengthBits = 0; // reset write pointer
			msg.Write((uint)unEncLenBits);
			msg.Write(buffer.AsSpan(0, (int) ms.Length));
			msg.LengthBits = newLength;

			return true;
		}

		public override bool Decrypt(NetIncomingMessage msg)
		{
			int unEncLenBits = (int)msg.ReadUInt32();

			var ms = new MemoryStream(msg.m_data, 4, msg.LengthBytes - 4);
			var cs = new CryptoStream(ms, m_algorithm.CreateDecryptor(), CryptoStreamMode.Read);

			var byteLen = NetUtility.BytesToHoldBits(unEncLenBits);
			var result = m_peer.GetStorage(byteLen);
			var read = 0;
			while (read < byteLen)
			{
				var cRead = cs.Read(result, read, byteLen-read);
				if (cRead == 0)
					return false;

				read += cRead;
			}
			
			cs.Close();

			// TODO: recycle existing msg

			msg.m_data = result;
			msg.m_bitLength = unEncLenBits;
			msg.m_readPosition = 0;

			return true;
		}
	}
}
