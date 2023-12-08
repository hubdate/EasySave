using EasySave.src.Utils;
using System;
using System.Diagnostics;

namespace EasySave.src.Models.Data {

    public class FullSave : Save {

        protected internal FullSave(
            string name,
            string src,
            string dst,
            Guid guid,
            JobStatus status = JobStatus.WAITING
        ) : base(name, src, dst, guid, status) { }

        public override SaveType GetSaveType() {
            return SaveType.FULL;
        }

        public override string ToString() {
            return $"{this.GetName()} - {this._uuid} | Full Save";
        }
    }
}