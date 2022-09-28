﻿namespace Minecraft.Terrain
{
    internal enum BlockType
    {
        Air, Grass, Stone, Dirt, Leaves, OakTrunk, Glass, Sand, Water, Bedrock, Cobblestone
    }
    internal static class BlockData
    {
        public static bool IsBlockSolid(int type)
        {
            return (BlockType)type != BlockType.Water || (BlockType)type != BlockType.Air;
        }
        public static bool IsBolckTransparent(int type)
        {
            return (BlockType)type == BlockType.Water ||
                   (BlockType)type == BlockType.Glass ||
                   (BlockType)type == BlockType.Air;
        }
    }
}