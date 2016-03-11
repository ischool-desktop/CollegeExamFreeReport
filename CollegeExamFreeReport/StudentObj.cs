﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollegeExamFreeReport
{
    class StudentObj
    {
        public string Id, Name, ClassName, IdNumber, SeatNo, StudentNumber;
        public decimal ServiceHours;
        public int CadreTimes;
        //原始獎懲紀錄
        public int MeritA, MeritB, MeritC, DemeritA, DemeritB, DemeritC;
        //功過相抵後獎懲紀錄
        public int MA, MB, MC, DA, DB, DC;
        //體適能常模
        public string sit_and_reach_degree, standing_long_jump_degree, sit_up_degree, cardiorespiratory_degree;

        public Dictionary<string, Dictionary<string, decimal>> DomainScores;
        private Dictionary<string, decimal> domainAverageScores;

        public List<string> TagIds;

        public Dictionary<string, decimal> GetDomainScores()
        {
            domainAverageScores.Clear();

            foreach (string domain in DomainScores.Keys)
            {
                int count = 0;

                if (!domainAverageScores.ContainsKey(domain))
                {
                    domainAverageScores.Add(domain, 0);
                }

                foreach (KeyValuePair<string, decimal> kvp in DomainScores[domain])
                {
                    if (kvp.Key == "1上" || kvp.Key == "1下" || kvp.Key == "2上" || kvp.Key == "2下" || kvp.Key == "3上" || kvp.Key == "7上" || kvp.Key == "7下" || kvp.Key == "8上" || kvp.Key == "8下" || kvp.Key == "9上")
                    {
                        domainAverageScores[domain] += kvp.Value;
                        count++;
                    }
                }

                if (count > 0)
                    domainAverageScores[domain] /= count;

                domainAverageScores[domain] = Math.Round(domainAverageScores[domain], 2, MidpointRounding.AwayFromZero);
            }

            return domainAverageScores;
        }

        public int DomainItemScore
        {
            get
            {
                int score = 0;

                if (domainAverageScores.ContainsKey("健康與體育"))
                {
                    if (domainAverageScores["健康與體育"] >= 60)
                    {
                        score += 2;
                    }
                }

                if (domainAverageScores.ContainsKey("藝術與人文"))
                {
                    if (domainAverageScores["藝術與人文"] >= 60)
                    {
                        score += 2;
                    }
                }

                if (domainAverageScores.ContainsKey("綜合活動"))
                {
                    if (domainAverageScores["綜合活動"] >= 60)
                    {
                        score += 2;
                    }
                }

                return score;
            }
        }


        public int ServiceHoursScore
        {
            get
            {
                int score = (int)(ServiceHours / 8);
                score += CadreTimes;

                if (score > 7)
                    score = 7;

                return score;
            }
        }

        public int MeritDemeritScore
        {
            get
            {
                int score = 0;

                if (DA == 0 && DB == 0 && DC == 0)
                {
                    score = 1;
                }

                if (!HasDemeritAB)
                {
                    if (MA > 0)
                    {
                        score = 4;
                    }
                    else if (MB > 0)
                    {
                        score = 3;
                    }
                    else if (MC > 0)
                    {
                        score = 2;
                    }
                }

                return score;
            }
        }

        public bool HasDemeritAB
        {
            get
            {
                if (DemeritA == 0 && DemeritB == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public int SportFitnessScore
        {
            get
            {
                int score = 0;
                //sit_and_reach_degree
                if (sit_and_reach_degree == "金牌" || sit_and_reach_degree == "銀牌" || sit_and_reach_degree == "銅牌" || sit_and_reach_degree == "中等")
                {
                    score += 2;
                }
                //standing_long_jump_degree
                if (standing_long_jump_degree == "金牌" || standing_long_jump_degree == "銀牌" || standing_long_jump_degree == "銅牌" || standing_long_jump_degree == "中等")
                {
                    score += 2;
                }
                //sit_up_degree
                if (sit_up_degree == "金牌" || sit_up_degree == "銀牌" || sit_up_degree == "銅牌" || sit_up_degree == "中等")
                {
                    score += 2;
                }
                //cardiorespiratory_degree
                if (cardiorespiratory_degree == "金牌" || cardiorespiratory_degree == "銀牌" || cardiorespiratory_degree == "銅牌" || cardiorespiratory_degree == "中等")
                {
                    score += 2;
                }

                if (score > 6)
                    score = 6;

                // 免測處理
                if (sit_and_reach_degree == "免測" || standing_long_jump_degree == "免測" || sit_up_degree == "免測" || cardiorespiratory_degree == "免測")
                {
                    score = 6;
                }

                return score;
            }
        }

        public string CheckScore(string str)
        {
            string value = "";
            switch (str)
            {
                case "坐姿體前彎":
                    value = sit_and_reach_degree;
                    break;

                case "立定跳遠":
                    value = standing_long_jump_degree;
                    break;

                case "仰臥起坐":
                    value = sit_up_degree;
                    break;

                case "心肺適能":
                    value = cardiorespiratory_degree;
                    break;

                default:
                    value = "";
                    break;
            }

            if (value == "金牌" || value == "銀牌" || value == "銅牌" || value == "中等" || value=="免測")
            {
                return "達";
            }            
            else
            {
                return "未達";
            }
        }

        public void MeritDemeritTransfer()
        {
            int merit = ((MeritA * Report.MAB) + MeritB) * Report.MBC + MeritC;
            int demerit = ((DemeritA * Report.DAB) + DemeritB) * Report.DBC + DemeritC;

            int total = merit - demerit;

            if (total > 0)
            {
                MC = total % Report.MBC;
                MB = (total / Report.MBC) % Report.MAB;
                MA = (total / Report.MBC) / Report.MAB;
                /*
                //最小單位先存起來
                MC = total;

                //原始紀錄有大功或小功必須先轉換一次
                if (MeritA > 0 || MeritB > 0)
                {
                    MB = MC / Report.MBC;
                    MC = MC % Report.MBC;
                }

                //原始紀錄有大功再轉一次
                if (MeritA > 0)
                {
                    MA = MB / Report.MAB;
                    MB = MB % Report.MAB;
                }
                 * */
            }
            else if (total < 0)
            {
                total *= -1;
                DC = total % Report.DBC;
                DB = (total / Report.DBC) % Report.DAB;
                DA = (total / Report.DBC) / Report.DAB;
            }
        }

        public StudentObj(DataRow row)
        {
            this.Id = row["id"].ToString();
            this.Name = row["name"].ToString();
            this.ClassName = row["class_name"].ToString();
            this.IdNumber = row["id_number"].ToString();
            this.SeatNo = row["seat_no"].ToString();
            this.StudentNumber = row["student_number"].ToString();
            this.ServiceHours = 0;
            this.CadreTimes = 0;
            this.MeritA = 0;
            this.MeritB = 0;
            this.MeritC = 0;
            this.DemeritA = 0;
            this.DemeritB = 0;
            this.DemeritC = 0;
            this.MA = 0;
            this.MB = 0;
            this.MC = 0;
            this.DA = 0;
            this.DB = 0;
            this.DC = 0;
            sit_and_reach_degree = "";
            standing_long_jump_degree = "";
            sit_up_degree = "";
            cardiorespiratory_degree = "";
            DomainScores = new Dictionary<string, Dictionary<string, decimal>>();
            domainAverageScores = new Dictionary<string, decimal>();
            TagIds = new List<string>();
        }
    }
}
