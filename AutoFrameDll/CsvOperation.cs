﻿using System;
using System.Text;
using System.Collections;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Diagnostics;
using CommonTool;

namespace AutoFrameDll
{
    /// <summary>
    /// 
    /// </summary>
    public class CsvOperation
    {
        private ArrayList rowAL;        //行链表,CSV文件的每一行就是一个链
        private string fileName;       //文件名
        private Encoding encoding;     //编码

        /// <summary>
        /// 默认以系统参数路径，当前日期生成文件名
        /// </summary>
        public CsvOperation()
        {
            this.rowAL = new ArrayList();
            this.fileName = SystemMgr.GetInstance().GetDataPath() + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            this.encoding = Encoding.Default;
        }

        /// <summary>
        ///以系统参数路径，文件名称及当前日期组合生成文件名
        /// </summary>
        /// <param name="fileName">文件名,包括文件路径</param>
        public CsvOperation(string fileName)
        {
            this.rowAL = new ArrayList();
            this.fileName = SystemMgr.GetInstance().GetDataPath() + "\\" + fileName + "_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            //  this.fileName = fileName;
            this.encoding = Encoding.Default;
        }

        /// <summary>
        ///指定文件名和编码方式
        /// </summary>
        /// <param name="fileName">文件名,包括文件路径</param>
        /// <param name="encoding">文件编码</param>
        public CsvOperation(string fileName, Encoding encoding)
        {
            this.rowAL = new ArrayList();
            this.fileName = SystemMgr.GetInstance().GetDataPath() + "\\" + fileName + "_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            //     this.fileName = fileName;
            this.encoding = encoding;
        }

        /// <summary>
        /// 文件名,包括文件路径
        /// </summary>
        public string FileName
        {
            set
            {
                this.fileName = value;
            }
        }

        /// <summary>
        /// 文件编码
        /// </summary>

        public Encoding FileEncoding
        {
            set
            {
                this.encoding = value;
            }
        }
        /// <summary>
        /// 获取行数
        /// </summary>
        public int RowCount
        {
            get
            {
                return this.rowAL.Count;
            }
        }
        /// <summary>
        /// 获取列数
        /// </summary>
        public int ColCount
        {
            get
            {
                int maxCol;
                maxCol = 0;
                for (int i = 0; i < this.rowAL.Count; i++)
                {
                    ArrayList colAL = (ArrayList)this.rowAL[i];

                    maxCol = (maxCol > colAL.Count) ? maxCol : colAL.Count;
                }
                return maxCol;
            }
        }

        /// <summary>
        /// 获取某行某列的数据
        /// row:行,row = 1代表第一行
        /// col:列,col = 1代表第一列  
        /// </summary>
        public string this[int row, int col]
        {
            get
            {
                //数据有效性验证
                CheckRowValid(row);
                CheckColValid(col);
                ArrayList colAL = (ArrayList)this.rowAL[row];
                //如果请求列数据大于当前行的列时,返回空值
                if (colAL.Count <= col)
                {
                    return "";
                }
                return colAL[col].ToString();
            }
            set
            {
                //对行进行判断
                if (row < 0)
                {
                    throw new Exception("行数不能小于0");
                }
                else if (row >= this.rowAL.Count) //如果当前列链的行数不够，要补齐
                {
                    for (int i = this.rowAL.Count; i <= row; i++)
                    {
                        this.rowAL.Add(new ArrayList());
                    }
                }

                //对列进行判断
                if (col < 0)
                {
                    throw new Exception("列数不能小于0");
                }
                else
                {
                    ArrayList colTempAL = (ArrayList)this.rowAL[row];
                    //扩大长度
                    if (col >= colTempAL.Count)
                    {
                        for (int i = colTempAL.Count; i <= col; i++)
                        {
                            colTempAL.Add("");
                        }
                    }
                    this.rowAL[row] = colTempAL;
                }
                //赋值
                ArrayList colAL = (ArrayList)this.rowAL[row];

                colAL[col] = value;
                this.rowAL[row] = colAL;
            }
        }

        /// <summary>
        /// 根据最小行，最大行，最小列，最大列，来生成一个DataTable类型的数据
        /// 行等于1代表第一行
        /// 列等于1代表第一列
        /// maxrow: -1代表最大行
        /// maxcol: -1代表最大列
        /// </summary>
        public DataTable this[int minRow, int maxRow, int minCol, int maxCol]
        {
            get
            {
                //数据有效性验证
                CheckRowValid(minRow);
                CheckMaxRowValid(maxRow);
                CheckColValid(minCol);
                CheckMaxColValid(maxCol);
                if (maxRow == -1)
                {
                    maxRow = RowCount;
                }
                if (maxCol == -1)
                {
                    maxCol = ColCount;
                }
                if (maxRow < minRow)
                {
                    throw new Exception("最大行数不能小于最小行数");
                }
                if (maxCol < minCol)
                {
                    throw new Exception("最大列数不能小于最小列数");
                }
                DataTable csvDT = new DataTable();
                int i;
                int col;
                int row;

                //增加列

                for (i = minCol; i <= maxCol; i++)
                {
                    csvDT.Columns.Add(i.ToString());
                }
                for (row = minRow; row <= maxRow; row++)
                {
                    DataRow csvDR = csvDT.NewRow();

                    i = 0;
                    for (col = minCol; col <= maxCol; col++)
                    {
                        csvDR[i] = this[row, col];
                        i++;
                    }
                    csvDT.Rows.Add(csvDR);
                }

                return csvDT;
            }
        }

        /// <summary>
        /// 检查行数是否是有效的
        /// </summary>
        /// <param name="row"></param>  
        private void CheckRowValid(int row)
        {
            if (row <= 0)
            {
                throw new Exception("行数不能小于0");
            }
            if (row > RowCount)
            {
                throw new Exception("没有当前行的数据");
            }
        }

        /// <summary>
        /// 检查最大行数是否是有效的
        /// </summary>
        /// <param name="maxRow"></param>  
        private void CheckMaxRowValid(int maxRow)
        {
            if (maxRow <= 0 && maxRow != -1)
            {
                throw new Exception("行数不能等于0或小于-1");
            }
            if (maxRow > RowCount)
            {
                throw new Exception("没有当前行的数据");
            }
        }

        /// <summary>
        /// 检查列数是否是有效的
        /// </summary>
        /// <param name="col"></param>  
        private void CheckColValid(int col)
        {
            if (col <= 0)
            {
                throw new Exception("列数不能小于0");
            }
            if (col > ColCount)
            {
                throw new Exception("没有当前列的数据");
            }
        }

        /// <summary>
        /// 检查检查最大列数是否是有效的
        /// </summary>
        /// <param name="maxCol"></param>  
        private void CheckMaxColValid(int maxCol)
        {
            if (maxCol <= 0 && maxCol != -1)
            {
                throw new Exception("列数不能等于0或小于-1");
            }
            if (maxCol > ColCount)
            {
                throw new Exception("没有当前列的数据");
            }
        }

        /// <summary>
        /// 载入CSV文件
        /// </summary>
        private void LoadCsvFile()
        {
            //对数据的有效性进行验证

            if (this.fileName == null)
            {
                throw new Exception("请指定要载入的CSV文件名");
            }
            else if (!File.Exists(this.fileName))
            {
                throw new Exception("指定的CSV文件不存在");
            }
            else
            {
            }
            if (this.encoding == null)
            {
                this.encoding = Encoding.Default;
            }

            string csvDataLine = "";

            using (FileStream fs = new FileStream(this.fileName, FileMode.Append, FileAccess.Write, FileShare.Write))
            {
                using (StreamReader reader = new StreamReader(fs, this.encoding))
                {
                    while (true)
                    {
                        string fileDataLine;

                        fileDataLine = reader.ReadLine();
                        if (fileDataLine == null)
                        {
                            break;
                        }
                        if (csvDataLine == "")
                        {
                            csvDataLine = fileDataLine;//GetDeleteQuotaDataLine(fileDataLine);
                        }
                        else
                        {
                            csvDataLine += "/r/n" + fileDataLine;//GetDeleteQuotaDataLine(fileDataLine);
                        }
                        //如果包含偶数个引号，说明该行数据中出现回车符或包含逗号
                        if (!IfOddQuota(csvDataLine))
                        {
                            AddNewDataLine(csvDataLine);
                            csvDataLine = "";
                        }
                    }

                }
            }
            //数据行出现奇数个引号
            if (csvDataLine.Length > 0)
            {
                throw new Exception("CSV文件的格式有错误");
            }
        }

        /// <summary>
        /// 获取两个连续引号变成单个引号的数据行
        /// </summary>
        /// <param name="fileDataLine">文件数据行</param>
        /// <returns></returns>
        private string GetDeleteQuotaDataLine(string fileDataLine)
        {
            return fileDataLine.Replace("\"\"", "\"");
        }

        /// <summary>
        /// 判断字符串是否包含奇数个引号
        /// </summary>
        /// <param name="dataLine">数据行</param>
        /// <returns>为奇数时，返回为真；否则返回为假</returns>
        private bool IfOddQuota(string dataLine)
        {
            int quotaCount;
            bool oddQuota;

            quotaCount = 0;
            for (int i = 0; i < dataLine.Length; i++)
            {
                if (dataLine[i] == '\"')
                {
                    quotaCount++;
                }
            }
            oddQuota = false;
            if (quotaCount % 2 == 1)
            {
                oddQuota = true;
            }
            return oddQuota;
        }

        /// <summary>
        /// 判断是否以奇数个引号开始
        /// </summary>
        /// <param name="dataCell"></param>
        /// <returns></returns>
        private bool IfOddStartQuota(string dataCell)
        {
            int quotaCount;
            bool oddQuota;

            quotaCount = 0;
            for (int i = 0; i < dataCell.Length; i++)
            {
                if (dataCell[i] == '\"')
                {
                    quotaCount++;
                }
                else
                {
                    break;
                }
            }
            oddQuota = false;
            if (quotaCount % 2 == 1)
            {
                oddQuota = true;
            }

            return oddQuota;
        }

        /// <summary>
        /// 判断是否以奇数个引号结尾
        /// </summary>
        /// <param name="dataCell"></param>
        /// <returns></returns>
        private bool IfOddEndQuota(string dataCell)
        {
            int quotaCount;
            bool oddQuota;

            quotaCount = 0;
            for (int i = dataCell.Length - 1; i >= 0; i--)
            {
                if (dataCell[i] == '\"')
                {
                    quotaCount++;
                }
                else
                {
                    break;
                }
            }
            oddQuota = false;
            if (quotaCount % 2 == 1)
            {
                oddQuota = true;
            }

            return oddQuota;
        }

        /// <summary>
        /// 加入新的数据行
        /// </summary>
        /// <param name="newDataLine">新的数据行</param>
        private void AddNewDataLine(string newDataLine)
        {
            Debug.WriteLine("NewLine:" + newDataLine);
            ArrayList colAL = new ArrayList();
            string[] dataArray = newDataLine.Split(',');
            bool oddStartQuota;       //是否以奇数个引号开始

            string cellData;
            oddStartQuota = false;
            cellData = "";
            for (int i = 0; i < dataArray.Length; i++)
            {
                if (oddStartQuota)
                {
                    //因为前面用逗号分割,所以要加上逗号
                    cellData += "," + dataArray[i];
                    //是否以奇数个引号结尾
                    if (IfOddEndQuota(dataArray[i]))
                    {
                        colAL.Add(GetHandleData(cellData));
                        oddStartQuota = false;
                        continue;
                    }
                }
                else
                {
                    //是否以奇数个引号开始

                    if (IfOddStartQuota(dataArray[i]))
                    {
                        //是否以奇数个引号结尾,不能是一个双引号,并且不是奇数个引号

                        if (IfOddEndQuota(dataArray[i]) && dataArray[i].Length > 2 && !IfOddQuota(dataArray[i]))
                        {
                            colAL.Add(GetHandleData(dataArray[i]));
                            oddStartQuota = false;
                            continue;
                        }
                        else
                        {

                            oddStartQuota = true;
                            cellData = dataArray[i];
                            continue;
                        }
                    }
                    else
                    {
                        colAL.Add(GetHandleData(dataArray[i]));
                    }
                }
            }
            if (oddStartQuota)
            {
                throw new Exception("数据格式有问题");
            }
            this.rowAL.Add(colAL);
        }


        /// <summary>
        /// 去掉格子的首尾引号，把双引号变成单引号
        /// </summary>
        /// <param name="fileCellData"></param>
        /// <returns></returns>
        private string GetHandleData(string fileCellData)
        {
            if (fileCellData == "")
            {
                return "";
            }
            if (IfOddStartQuota(fileCellData))
            {
                if (IfOddEndQuota(fileCellData))
                {
                    return fileCellData.Substring(1, fileCellData.Length - 2).Replace("\"\"", "\""); //去掉首尾引号，然后把双引号变成单引号
                }
                else
                {
                    throw new Exception("数据引号无法匹配" + fileCellData);
                }
            }
            else
            {
                //考虑形如""    """"      """"""   
                if (fileCellData.Length > 2 && fileCellData[0] == '\"')
                {
                    fileCellData = fileCellData.Substring(1, fileCellData.Length - 2).Replace("\"\"", "\""); //去掉首尾引号，然后把双引号变成单引号
                }
            }

            return fileCellData;
        }

        /// <summary>
        /// 添加表数据到CSV文件中
        /// </summary>
        /// <param name="dataDT">表数据</param>
        /// <param name="beginCol">从第几列开始,beginCol = 1代表第一列</param>
        public void AddData(DataTable dataDT, int beginCol)
        {
            if (dataDT == null)
            {
                throw new Exception("需要添加的表数据为空");
            }
            int curMaxRow;

            curMaxRow = this.rowAL.Count;
            for (int i = 0; i < dataDT.Rows.Count; i++)
            {
                for (int j = 0; j < dataDT.Columns.Count; j++)
                {
                    this[curMaxRow + i + 1, beginCol + j] = dataDT.Rows[i][j].ToString();
                }
            }
        }

        /// <summary>
        /// 保存数据,如果当前硬盘中已经存在文件名一样的文件，将会覆盖
        /// </summary>
        public void Save()
        {
            //对数据的有效性进行判断
            if (this.fileName == null)
            {
                throw new Exception("缺少文件名");
            }
            if (this.encoding == null)
            {
                this.encoding = Encoding.Default;
            }
            try
            {


                using (FileStream fs = new FileStream(this.fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter writer = new StreamWriter(fs, this.encoding))
                    {
                        for (int i = 0; i < this.rowAL.Count; i++)
                        {
                            writer.WriteLine(ConvertToSaveLine((ArrayList)this.rowAL[i]));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString(), "文件写入失败");
            }

        }

        /// <summary>
        /// 保存数据,如果当前硬盘中已经存在文件名一样的文件，将会覆盖
        /// </summary>
        /// <param name="fileName">文件名,包括文件路径</param>
        public void Save(string fileName)
        {
            this.fileName = fileName;
            Save();
        }

        /// <summary>
        /// 保存数据,如果当前硬盘中已经存在文件名一样的文件，将会覆盖
        /// </summary>
        /// <param name="fileName">文件名,包括文件路径</param>
        /// <param name="encoding">文件编码</param>
        public void Save(string fileName, Encoding encoding)
        {
            this.fileName = fileName;
            this.encoding = encoding;
            Save();
        }


        /// <summary>
        /// 转换成保存行
        /// </summary>
        /// <param name="colAL">一行</param>
        /// <returns></returns>
        private string ConvertToSaveLine(ArrayList colAL)
        {
            string saveLine;

            saveLine = "";
            for (int i = 0; i < colAL.Count; i++)
            {
                saveLine += ConvertToSaveCell(colAL[i].ToString());
                //格子间以逗号分割
                if (i < colAL.Count - 1)
                {
                    saveLine += ",";
                }
            }
            return saveLine;
        }

        /// <summary>
        /// 字符串转换成CSV中的格子
        /// 双引号转换成两个双引号，然后首尾各加一个双引号
        /// 这样就不需要考虑逗号及换行的问题
        /// </summary>
        /// <param name="cell">格子内容</param>
        /// <returns></returns>
        private string ConvertToSaveCell(string cell)
        {
            cell = cell.Replace("\"", "\"\"");

            return "\"" + cell + "\"";
        }
    }
}
