namespace Astringent.Game20220410.Scripts
{
   /* static class UnsafeEuqaler
    {
        public unsafe static bool Equal<T>(in T t1, in T t2) where T : unmanaged
        {

            unsafe
            {
                var size = sizeof(T);
                fixed(T* ptr1 = &t1)
                {
                    
                    fixed (T* ptr2 = &t2)
                    {
                        byte* b1 = (byte*)ptr1;
                        byte* b2 = (byte*)ptr2;
                        for (int i = 0; i < size; i++)
                        {
                            if(b1[i] != b2[i])
                                return false;
                        }
                    }
                }
            }
            return true;
        }
    }*/

}

