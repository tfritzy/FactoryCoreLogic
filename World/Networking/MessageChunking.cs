using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public static class MessageChunker
    {
        public const int ChunkSize = 512;
        private static byte[] buffer = new byte[ChunkSize];

        public static List<Schema.UpdatePacket> Chunk(List<byte[]> updates)
        {
            List<Schema.UpdatePacket> packets = new List<Schema.UpdatePacket>();
            Schema.UpdatePacket currentPacket = new Schema.UpdatePacket();
            int currentPacketSize = 0;

            foreach (byte[] update in updates)
            {
                int totalChunks = (int)Math.Ceiling((double)update.Length / ChunkSize);
                for (int i = 0; i < totalChunks; i++)
                {
                    int chunkSize = Math.Min(ChunkSize, update.Length - i * ChunkSize);

                    Buffer.BlockCopy(update, i * ChunkSize, buffer, 0, chunkSize);

                    var chunk = new Schema.Chunk
                    {
                        Index = i,
                        MaxIndex = totalChunks - 1,
                        Data = Google.Protobuf.ByteString.CopyFrom(buffer, 0, chunkSize)
                    };

                    if (currentPacketSize + chunkSize > ChunkSize)
                    {
                        packets.Add(currentPacket);
                        currentPacket = new Schema.UpdatePacket();
                        currentPacketSize = 0;
                    }

                    currentPacket.Chunks.Add(chunk);
                    currentPacketSize += chunkSize;
                }
            }

            if (currentPacket.Chunks.Count > 0)
            {
                packets.Add(currentPacket);
            }

            return packets;
        }

        public static byte[]? ExtractFullUpdate(ref List<Schema.UpdatePacket> packets)
        {
            int? updateEndPacketIndex = null;
            int? updateEndChunkIndex = null;

            bool shouldBail = false;
            for (int i = 0; i < packets.Count; i++)
            {
                for (int c = 0; c < packets[i].Chunks.Count; c++)
                {
                    if (packets[i].Chunks[c].Index == packets[i].Chunks[c].MaxIndex)
                    {
                        updateEndPacketIndex = i;
                        updateEndChunkIndex = c;
                        shouldBail = true;
                        break;
                    }
                }

                if (shouldBail)
                {
                    break;
                }
            }

            if (updateEndPacketIndex == null || updateEndChunkIndex == null)
            {
                return null;
            }

            List<Schema.Chunk> chunks = new List<Schema.Chunk>();
            for (int i = 0; i <= updateEndPacketIndex; i++)
            {
                if (i < updateEndPacketIndex)
                {
                    chunks.AddRange(packets[i].Chunks);
                }
                else if (i == updateEndPacketIndex)
                {
                    chunks.AddRange(packets[i].Chunks.Take((int)updateEndChunkIndex + 1));

                    for (int j = 0; j <= updateEndChunkIndex; j++)
                    {
                        packets[i].Chunks.RemoveAt(0);
                    }
                }
            }
            int numPacketsToRemove =
                (int)updateEndPacketIndex +
                (packets[updateEndPacketIndex.Value].Chunks.Count == 0 ? 1 : 0);
            packets.RemoveRange(0, numPacketsToRemove);

            byte[] data = ReassembleUpdateData(chunks);
            return data;
        }

        private static byte[] ReassembleUpdateData(List<Schema.Chunk> chunks)
        {
            int totalSize = chunks.Sum(c => c.Data.Length);
            byte[] data = new byte[totalSize];

            int currentPosition = 0;
            foreach (var chunk in chunks.OrderBy(c => c.Index))
            {
                var chunkBytes = chunk.Data.ToByteArray();
                Buffer.BlockCopy(chunkBytes, 0, data, currentPosition, chunkBytes.Length);
                currentPosition += chunkBytes.Length;
            }

            return data;
        }
    }
}