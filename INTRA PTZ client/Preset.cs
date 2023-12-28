using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static INTRA_PTZ_client.Route;

namespace INTRA_PTZ_client
{
    [Serializable]
    public class Preset
    {
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

        public List<PresetTableRow> GetPresetList()
        {
            return presetList;
        }
        public void SetPresetList(List<PresetTableRow> presetList)
        {
            this.presetList = presetList;
        }

        public Preset()
        {
            for (int i = 1; i <= 20; i++)
            {
                presetList.Add(new PresetTableRow(i, 0, 0));
            }
        }

        public void AddRow()
        {
            //TODO
            List<PresetTableRow> newPresetList = new List<PresetTableRow>();
            for (int i = 0; i < presetList.Count; i++)
            {
                newPresetList.Add(presetList[i]);
            }
            newPresetList.Add(new PresetTableRow(presetList.Count + 1, 0, 0));
            SetPresetList(newPresetList);
        }

        public void DeleteRow()
        {
            //TODO
            if (presetList.Count > 0)
            {
                List<PresetTableRow> newPresetList = new List<PresetTableRow>();
                for (int i = 0; i < presetList.Count - 1; i++)
                {
                    newPresetList.Add(presetList[i]);
                }
                SetPresetList(newPresetList);
            }
        }

        public String getTooltipDesc(int num)
        {
            if (num > presetList.Count - 1)
            {
                return ",";
            }
            else
            {
                return presetList[num].Pan + ", " + presetList[num].Tilt;
            }
        }
    }
}
