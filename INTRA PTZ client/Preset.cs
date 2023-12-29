using System;
using System.Collections.Generic;

namespace INTRA_PTZ_client
{
    [Serializable]
    public class Preset
    {
        [Serializable]
        public class PresetTableRow
        {
            private int count = 0;
            private float pan = 0;
            private float tilt = 0;
            //private float focus = 0;
            //private float zoom = 0;            

            public PresetTableRow(int count, float pan, float tilt)
            {
                this.count = count;
                this.pan = pan;
                this.tilt = tilt;
            }
            public int Count { get => count; set => count = value; }
            public float Pan { get => pan; set => pan = value; }
            public float Tilt { get => tilt; set => tilt = value; }

            public override string ToString()
            {
                return "count = " + count + " pan = " + pan + " tilt = " + tilt;
            }
        }

        private List<PresetTableRow> presetList = new List<PresetTableRow>();

        public List<PresetTableRow> PresetList { get => presetList; set => presetList = value; }

        public List<PresetTableRow> GetPresetList()
        {
            return PresetList;
        }
        public void SetPresetList(List<PresetTableRow> presetList)
        {
            this.PresetList = presetList;
        }

        public Preset()
        {/*
            if (PresetList.Count == 0)
            {
                for (int i = 1; i <= 20; i++)
                {
                    PresetList.Add(new PresetTableRow(i, 0, 0));
                }
            }
            */
        }

        public void AddRow()
        {
            //TODO
            List<PresetTableRow> newPresetList = new List<PresetTableRow>();
            for (int i = 0; i < PresetList.Count; i++)
            {
                newPresetList.Add(PresetList[i]);
            }
            newPresetList.Add(new PresetTableRow(PresetList.Count + 1, 0, 0));
            SetPresetList(newPresetList);
        }

        public void DeleteRow()
        {
            //TODO
            if (PresetList.Count > 0)
            {
                List<PresetTableRow> newPresetList = new List<PresetTableRow>();
                for (int i = 0; i < PresetList.Count - 1; i++)
                {
                    newPresetList.Add(PresetList[i]);
                }
                SetPresetList(newPresetList);
            }
        }

        public String getTooltipDesc(int num)
        {
            if (num > PresetList.Count - 1)
            {
                return ",";
            }
            else
            {
                return PresetList[num].Pan + ", " + PresetList[num].Tilt;
            }
        }
    }
}
