﻿namespace Prediction.Core.Solvers
{
    public enum SolverTypeEnum
    {
        None = 0,
        DP = 1,
        BP = 2,
        DPDI = 4,
        BPDI = 8,
        Maher = 16,
        DPG = 32,
        BPG = 64,
        DPGDI = 128,
        BPGDI = 256
    }
}
