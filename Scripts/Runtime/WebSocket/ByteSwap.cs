
    public static class ByteSwap
    {
        // 16位高低字节交换
        public static ushort Swap16(ushort value)
        {
            return (ushort)(((value & 0xFF00) >> 8) |
                            ((value & 0x00FF) << 8));
        }

        // 32位高低字节交换
        public static uint Swap32(uint value)
        {
            return ((value & 0xFF000000) >> 24) |
                   ((value & 0x00FF0000) >> 8) |
                   ((value & 0x0000FF00) << 8) |
                   ((value & 0x000000FF) << 24);
        }
    }
