﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.SaveSystem.Data.Ammo
{
    internal class PersistentAmmoSlot
    {
        private List<CompoundDataPiece> data= new List<CompoundDataPiece>();

        public int Count => data.Count;

        public void UpdateFromExistingSlot(global::Ammo.Slot slot)
        {
            CompensateForExternalChanges(slot?.count ?? 0);
        }

        public CompoundDataPiece PopBottom()
        {
            var temp = data.First();
            data.Remove(temp);
            return temp;
        }

        public CompoundDataPiece PopTop()
        {
            var temp = data.Last();
            data.Remove(temp);
            return temp;
        }

        public CompoundDataPiece PeekTop()
        {
            return data.Last();
        }

        public void PushBottom(CompoundDataPiece piece)
        {
            data.Insert(0,piece);
        }

        public void PushTop(CompoundDataPiece piece)
        {
            data.Add(piece);
        }

        public void CompensateForExternalChanges(int realAmount)
        {
            if (realAmount - Count != 0)
            {
                Debug.Log("Compensating for an apparent ammo difference of "+(realAmount-Count));
            }

            while (realAmount > Count)
            {
                PushBottom(null);
            }

            while (realAmount < Count)
            {
                PopBottom();
            }

        }

        public bool HasNoData()
        {
            foreach(var v in data)
                if (v != null)
                    return false;
            return true;
        }


        public void Read(BinaryReader reader)
        {
            data.Clear();
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                bool shouldbenull = reader.ReadBoolean();
                if (shouldbenull)
                {
                    data.Add(null);
                }
                else
                {
                    var piece = DataPiece.Deserialize(reader) as CompoundDataPiece;
                    ExtendedData.CullMissingModsFromData(piece);
                    if (piece.DataList.Count == 0) piece = null;
                    data.Add(piece);
                }
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(data.Count);
            foreach (var piece in data)
            {
                writer.Write(piece==null);
                if (piece != null)
                {
                    piece.Serialize(writer);
                }
            }
        }

        
    }
}
