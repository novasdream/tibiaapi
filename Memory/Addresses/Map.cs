namespace Tibia.Memory.Addresses
{
    /// <summary>
    /// Map memory addresses.
    /// </summary>
    public static class Map
    {
        /// <summary>
        /// Pointer to the start of the map memory addresses.
        /// </summary>
        public static uint MapPointer = 0x61E408;     //8.0

        /// <summary>
        /// Map distances and offsets.
        /// </summary>
        public static uint Step_Tile = 172;          //8.0
        public static uint Step_Object = 0xC;
        public static uint Distance_Object_Id = 0;        //8.0
        public static uint Distance_Object_Data = 4;      //8.0
        public static uint Distance_Object_Data_Ex = 8;      //8.0

        /// <summary>
        /// Maximum number of tiles.
        /// </summary>
        public static uint Max_Tiles = 2016;

        /// <summary>
        /// Class of addresses and distances for level spy.
        /// </summary>
        public class LevelSpy
        {
            public static uint NOP = 0x004C4320;             //8.0 // writeNops 2 bytes; 2B C1: SUB EAX, ECX
            public static uint Above = 0x004C431C;           //8.0
            public static uint Below = 0x004C4324;           //8.0

            public static uint NOP_Default = 49451;          //8.0 // integer 2 bytes; 2B C1: SUB EAX, ECX
            public static uint Above_Default = 7;            //8.0
            public static uint Below_Default = 2;            //8.0

            public static uint NameSPY_NOP = 0x004DD2D7;     //8.0 // writeNops 2 bytes; 75 4C: JNZ SHORT
            public static uint NameSPY_NOP2 = 0x004DD2E1;    //8.0 // writeNops 2 bytes; 75 42: JNZ SHORT

            public static uint NameSPY_NOP_Default = 19573;  //8.0 // integer 2 bytes; 75 4C: JNZ SHORT
            public static uint NameSPY_NOP2_Default = 17013; //8.0 // integer 2 bytes; 75 42: JNZ SHORT

            public static uint MIN = 0; //8.0
            public static uint MAX = 7; //8.0

            public static uint Z_Axis_Default = 7; // default ground level
        }
    }
}
