using EasySave.src.Utils;
using System;
using System.Diagnostics;

namespace EasySave.src.Models.Data {

    public class DifferentialSave : Save {

        protected internal DifferentialSave(
            string name,
            string src,
            string dst,
            Guid guid,
            JobStatus status = JobStatus.WAITING
        ) : base(name, src, dst, guid, status) { }

        public override SaveType GetSaveType() {
            return SaveType.DIFFERENTIAL;
        }

        public override string ToString() {
            return $"{this.GetName()} - {this._uuid} | Differential Save";
        }


    }
}
