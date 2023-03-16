namespace IbkrToSru.TastyModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;

    public class TastyCSV
    {
        public int TAX_YEAR { get; set; }

        public string? SUBLOT_ID { get; set; }

        public string? SECNO { get; set; }

        public string? CUSIP { get; set; }

        public string? SYMBOL { get; set; }

        public string? SEC_DESCR { get; set; }

        public string? SEC_TYPE { get; set; }

        public string? SEC_SUBTYPE { get; set; }

        public string? SUBACCOUNT_TYPE { get; set; }

        public string? OPEN_TRAN_ID { get; set; }

        public string? CLOSE_TRAN_ID_SEQNO { get; set; }

        public DateTime OPEN_DATE { get; set; }

        public DateTime CLOSE_DATE { get; set; }

        public string? CLOSE_EVENT { get; set; }

        public string? DISPOSAL_METHOD { get; set; }

        public double QUANTITY { get; set; }

        public string? LONG_SHORT_IND { get; set; }

        public string? NO_WS_COST { get; set; }

        public string? NO_WS_PROCEEDS { get; set; }

        public string? NO_WS_GAINLOSS { get; set; }

        public string? WS_COST_ADJ { get; set; }

        public string? WS_PROC_ADJ { get; set; }

        public string? WS_LOSS_ID_SEQNO { get; set; }

        public string? num1099_ACQ_DATE { get; set; }

        public string? num1099_DISP_DATE { get; set; }

        public string? num1099_COST { get; set; }

        public string? num1099_PROCEEDS { get; set; }

        public string? GROSS_NET_IND { get; set; }

        public string? TOTAL_GAINLOSS { get; set; }

        public string? ORDINARY_GAINLOSS { get; set; }

        public string? num1099_DISALLOWED_LOSS { get; set; }

        public string? num1099_MARKET_DISCOUNT { get; set; }

        public string? num8949_GAINLOSS { get; set; }

        public string? num8949_CODE { get; set; }

        public string? HOLDING_DATE { get; set; }

        public string? TERM { get; set; }

        public string? COVERED_IND { get; set; }

        public string? num8949_BOX { get; set; }

        public string? num1099_1256_CY_REALIZED { get; set; }

        public string? num1099_1256_PY_UNREALIZED { get; set; }

        public string? num1099_1256_CY_UNREALIZED { get; set; }

        public string? num1099_1256_AGGREGATE { get; set; }
    }
}
