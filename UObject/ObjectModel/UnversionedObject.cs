using System;
using System.Collections.Generic;
using UObject.Asset;

namespace UObject.ObjectModel
{
    public class UnversionedObject : UnrealObject
    {
        private List<UnversionedFragment> Fragments { get; set; } = new List<UnversionedFragment>();
        private byte[] Mask { get; set; } = Array.Empty<byte>();
        private int FragmentIndex { get; set; }
        private int PropIndex { get; set; } = -1;
        private int PropsLeftInFragment { get; set; } = 0;
        private int MaskIndex { get; set; } = 0;
        
        public override void Deserialize(Span<byte> buffer, AssetFile asset, ref int cursor)
        {
            var fragment = new UnversionedFragment();
            var mask = 0;
            while (!fragment.IsLast)
            {
                fragment = new UnversionedFragment();
                fragment.Deserialize(buffer, asset, ref cursor);
                if (fragment.HasZero) mask += fragment.Count;
                Fragments.Add(fragment);
            }

            if (mask > 0)
            {
                Mask = mask switch
                {
                    <= 8 => buffer.Slice(cursor, 1).ToArray(),
                    <= 16 => buffer.Slice(cursor, 2).ToArray(),
                    _ => buffer.Slice(cursor, ((mask + 31) >> 5) * 4).ToArray()
                };
            }

            while (true)
            {
                var index = -1;
                var isZero = false;
            
                if (PropsLeftInFragment == 0)
                {
                    while (true)
                    {
                        if (++FragmentIndex == Fragments.Count)
                        {
                            return;
                        }

                        PropIndex += Fragments[FragmentIndex].Skip;
                        if (Fragments[FragmentIndex].Count <= 0) continue;
                        PropsLeftInFragment = Fragments[FragmentIndex].Count;
                        break;
                    }
                }

                if (Fragments[FragmentIndex].HasZero)
                {
                    isZero = (Mask[MaskIndex >> 3] & (1 << (MaskIndex & 7))) != 0;
                    MaskIndex++;
                }

                index = PropIndex++;
                PropsLeftInFragment--;
            }
        }
    }
}
