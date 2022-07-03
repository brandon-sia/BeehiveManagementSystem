using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeehiveManagementSystem
{
    abstract class Bee
    {
        
        public string Job { get; private set; }
        public Bee(string job)
        {
            Job = job;
        }
        protected abstract float CostPerShift { get; }

        protected abstract void DoJob();
        
        public void WorkTheNextShift()
        {
            if (HoneyVault.ConsumeHoney(CostPerShift))
            {
                DoJob();
            }
        }
    }

    class Queen : Bee
    {
        private Bee[] workers = new Bee[0];
        private float eggs = 0;
        private float unassignedWorkers = 3;
        private const float EGGS_PER_SHIFT = 0.45f;
        private const float HONEY_PER_UNASSIGNED_WORKER = 0.5f;
        public string StatusReport { get; private set; }
        protected override float CostPerShift { get { return 2.15f; } }

        public Queen() :base("Queen")
        {
            AssignBee("Egg Care");
            AssignBee("Honey Manufacturer");
            AssignBee("Nectar Collector");
        }

        public void CareForEggs(float eggsToConvert)
        {
            if (eggs >= eggsToConvert)
            {
                eggs -= eggsToConvert;
                unassignedWorkers += eggsToConvert;
            }
        }

        private void AddWorker(Bee worker)
        {
            if(unassignedWorkers >= 1)
            {
                unassignedWorkers--;
                Array.Resize(ref workers, workers.Length + 1);
                workers[workers.Length - 1] = worker;
            }
        }

        private void UpdateStatusReport()
        {
            StatusReport = $"Vault report:\n{HoneyVault.StatusReport}" +
            $"\nEgg count: {eggs:0.0}\nUnassigned workers: {unassignedWorkers:0.0}\n"+
            $"{WorkerStatus("Nectar Collector")}\n{WorkerStatus("Honey Manufacturer")}\n"+
            $"{WorkerStatus("Egg Care")}\nTOTAL WORKERS: {workers.Length}";
        }
        private string WorkerStatus(string job)
        {
            int count = 0;
            foreach(Bee worker in workers)
            {
                if (worker.Job == job) count++;
            }
            string s = "s";
            if (count == 1) s = "";
            return $"{count} {job} bee{s}";
        }

        protected override void DoJob()
        {
            eggs += EGGS_PER_SHIFT;
            foreach (Bee worker in workers)
            {
                worker.WorkTheNextShift();
            }
            HoneyVault.ConsumeHoney(HONEY_PER_UNASSIGNED_WORKER * workers.Length);
            UpdateStatusReport();
        }


        public void AssignBee(string job)
        {
            switch (job)
            {
                case "Egg Care":
                    AddWorker(new EggCare(this));
                    break;

                case "Honey Manufacturer":
                    AddWorker(new HoneyManufacturer());
                    break;

                case "Nectar Collector":
                    AddWorker(new NectarCollector());
                    break;

                default:
                    Console.WriteLine("Unknown job");
                    break;
            }

            UpdateStatusReport();
        }
    }
    class HoneyManufacturer : Bee
    {
        public HoneyManufacturer() : base("Honey Manufacturer") { }
        private const float NECTAR_PROCESSED_PER_SHIFT = 33.15f;
        protected override void DoJob()
        {
            HoneyVault.ConvertNectarToHoney(NECTAR_PROCESSED_PER_SHIFT);
        }
        protected override float CostPerShift { get { return 1.7f; } }
    }
    class NectarCollector : Bee

    {
        public NectarCollector() : base("Nectar Collector") { }
        private const float NECTAR_COLLECTED_PER_SHIFT = 33.25f;
        protected override void DoJob()
        {
            HoneyVault.CollectNectar(NECTAR_COLLECTED_PER_SHIFT);
        }
        protected override float CostPerShift { get { return 1.95f; } }
    }
    class EggCare : Bee
    {
        private const float CARE_PROGRESS_PER_SHIFT = 0.15f;
        protected override float CostPerShift { get { return 1.35f; } }

        private Queen queen;

        public EggCare(Queen queen):base("Egg Care")
        {
            this.queen = queen;
        }
        protected override void DoJob()
        {
            queen.CareForEggs(CARE_PROGRESS_PER_SHIFT);
        }
        

    }
}
