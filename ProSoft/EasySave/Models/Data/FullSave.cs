using System;
using System.Diagnostics;

namespace EasySave.Models.Data {
    public class FullSave : Save {
        protected internal FullSave(
            string name,
            string src,
            string dst,
            Guid uuid,
            JobStatus status = JobStatus.WAITING
        ) : base(name, src, dst, uuid, status) { }

        public override SaveType GetSaveType() {
            return SaveType.FULL;
        }

        public override string ToString() {
            //[TO DO] Need to be changed with the Resource.SaveType.FULL
            return $"{GetName()} - {uuid} | Full Save";
        }
    }
}