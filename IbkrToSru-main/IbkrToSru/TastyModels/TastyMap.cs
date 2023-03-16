namespace IbkrToSru.TastyModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CsvHelper.Configuration;

    public class TastyClassMap : ClassMap<TastyCSV>
    {
        public TastyClassMap()
        {
            this.Map(m => m.TAX_YEAR).Index(0);
            this.Map(m => m.SUBLOT_ID).Index(1);
            this.Map(m => m.SECNO).Index(2);
            this.Map(m => m.CUSIP).Index(3);
            this.Map(m => m.SYMBOL).Index(4);
            this.Map(m => m.SEC_DESCR).Index(5);
            this.Map(m => m.SEC_TYPE).Index(6);
            this.Map(m => m.SEC_SUBTYPE).Index(7);
            this.Map(m => m.SUBACCOUNT_TYPE).Index(8);
            this.Map(m => m.OPEN_TRAN_ID).Index(9);
            this.Map(m => m.CLOSE_TRAN_ID_SEQNO).Index(10);
            this.Map(m => m.OPEN_DATE).Index(11);
            this.Map(m => m.CLOSE_DATE).Index(12);
            this.Map(m => m.CLOSE_EVENT).Index(13);
            this.Map(m => m.DISPOSAL_METHOD).Index(14);
            this.Map(m => m.QUANTITY).Index(15);
            this.Map(m => m.LONG_SHORT_IND).Index(16);
            this.Map(m => m.NO_WS_COST).Index(17);
            this.Map(m => m.NO_WS_PROCEEDS).Index(18);
            this.Map(m => m.NO_WS_GAINLOSS).Index(19);
            this.Map(m => m.WS_COST_ADJ).Index(20);
            this.Map(m => m.WS_PROC_ADJ).Index(21);
            this.Map(m => m.WS_LOSS_ID_SEQNO).Index(22);
            this.Map(m => m.num1099_ACQ_DATE).Index(23);
            this.Map(m => m.num1099_DISP_DATE).Index(24);
            this.Map(m => m.num1099_COST).Index(25);
            this.Map(m => m.num1099_PROCEEDS).Index(26);
            this.Map(m => m.GROSS_NET_IND).Index(27);
            this.Map(m => m.TOTAL_GAINLOSS).Index(28);
            this.Map(m => m.ORDINARY_GAINLOSS).Index(29);
            this.Map(m => m.num1099_DISALLOWED_LOSS).Index(30);
            this.Map(m => m.num1099_MARKET_DISCOUNT).Index(31);
            this.Map(m => m.num8949_GAINLOSS).Index(32);
            this.Map(m => m.num8949_CODE).Index(33);
            this.Map(m => m.HOLDING_DATE).Index(34);
            this.Map(m => m.TERM).Index(35);
            this.Map(m => m.COVERED_IND).Index(36);
            this.Map(m => m.num8949_BOX).Index(37);
            this.Map(m => m.num1099_1256_CY_REALIZED).Index(38);
            this.Map(m => m.num1099_1256_PY_UNREALIZED).Index(39);
            this.Map(m => m.num1099_1256_CY_UNREALIZED).Index(40);
            this.Map(m => m.num1099_1256_AGGREGATE).Index(41);
        }
    }
}
