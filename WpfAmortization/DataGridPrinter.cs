#region © 2020 JoeWare.
//
// All rights reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical, or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
#endregion

using System;
using System.Drawing;
using System.Windows;
using System.Collections;
using System.Drawing.Printing;
using System.Windows.Controls;

using Amortization;

namespace WpfAmortization
{
    // ----------------------------------------------------
    /// <summary>
    ///     DataGridPrinter Description
    /// </summary>

    public class DataGridPrinter
    {
        private Font _font;
        private DataGrid _dataGrid;
        private const int _cellMargin = 10;
        private PrintDocument _printDocument;

        public int _rowCount = 0;
        public int _pageNumber = 1;
        public ArrayList _lines = new ArrayList();

        private int _topMargin;
        private int _pageWidth;
        private int _columnwidth;
        private int _pPageHeight;
        private int _bottomMargin;

        private float _location = 10.0f;
        private Color _foreColor = Color.Black;
        private Color _backColor = Color.AliceBlue;
        private Color _altBackColor = Color.LightGray;
        private Color _headerForeColor = Color.Black;
        private Color _headerBackColor = Color.LightSkyBlue;

        // ------------------------------------------------

        public DataGridPrinter(DataGrid aGrid, PrintDocument aPrintDocument, Font font)
        {
            _font = font;
            _dataGrid = aGrid;
            _printDocument = aPrintDocument;

            _topMargin = _printDocument.DefaultPageSettings.Margins.Top;
            _bottomMargin = _printDocument.DefaultPageSettings.Margins.Bottom;
            _pPageHeight = _printDocument.DefaultPageSettings.PaperSize.Height;

            _pageWidth = _printDocument.DefaultPageSettings.PaperSize.Width;
            _columnwidth = _pageWidth / _dataGrid.Columns.Count;
        }

        // ------------------------------------------------

        void DrawHorizontalLines(Graphics graphics, ArrayList lines)
        {
            var linePen = new Pen(_backColor, 1);

            for(var i = 0; i < lines.Count; i++)
            {
                graphics.DrawLine(linePen, _location, (float)lines[i], _pageWidth, (float)lines[i]);
            }
        }

        // ------------------------------------------------

        void DrawVerticalGridLines(Graphics graphics, Pen TheLinePen, int columnwidth, int bottom)
        {
            for(var currentColumn = 0; currentColumn < _dataGrid.Columns.Count; currentColumn++)
            {
                graphics.DrawLine(TheLinePen,
                                  _location + currentColumn * columnwidth,
                                  _location + _topMargin,
                                  _location + currentColumn * columnwidth,
                                  bottom);
            }
        }

        // ------------------------------------------------

        public bool DrawDataGrid(Graphics graphics)
        {
            bool bContinue;

            try
            {
                DrawHeader(graphics);
                bContinue = DrawRows(graphics);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                bContinue = false;
            }

            return bContinue;
        }

        // ------------------------------------------------

        public void DrawHeader(Graphics graphics)
        {
            var initialRowCount = _rowCount;
            var cellformat = new StringFormat();
            var linePen = new Pen(_backColor, 1);
            var foreBrush = new SolidBrush(_headerForeColor);
            var backBrush = new SolidBrush(_headerBackColor);

            cellformat.Trimming = StringTrimming.EllipsisCharacter;
            cellformat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;

            // ---------------------
            // draw the table header

            var startxposition = _location;

            var HeaderBounds = new RectangleF(0, 0, 0, 0);
            var nextcellbounds = new RectangleF(0, 0, 0, 0);

            HeaderBounds.X = _location;
            HeaderBounds.Y = _location + _topMargin +
                             (_rowCount - initialRowCount) * (_font.SizeInPoints + _cellMargin);

            HeaderBounds.Height = _font.SizeInPoints + _cellMargin;
            HeaderBounds.Width = _pageWidth;

            graphics.FillRectangle(backBrush, HeaderBounds);

            foreach(var property in new PaymentDetail().GetProperties())
            {
                var cellbounds = new RectangleF(startxposition,
                                                _location + _topMargin +
                                                (_rowCount - initialRowCount) * (_font.SizeInPoints + _cellMargin),
                                                _columnwidth,
                                                _font.SizeInPoints + _cellMargin);
                nextcellbounds = cellbounds;

                if(startxposition + _columnwidth <= _pageWidth + 25)
                {
                    graphics.DrawString(property.Name, _font, foreBrush, cellbounds, cellformat);
                }

                startxposition = startxposition + _columnwidth;
            }

            graphics.DrawLine(linePen, _location, nextcellbounds.Bottom, _pageWidth, nextcellbounds.Bottom);
        }

        // ------------------------------------------------

        public bool DrawRows(Graphics graphics)
        {
            var lastRowBottom = _topMargin;

            try
            {
                var ForeBrush = new SolidBrush(_foreColor);
                var BackBrush = new SolidBrush(_backColor);
                var AlternatingBackBrush = new SolidBrush(_altBackColor);
                var TheLinePen = new Pen(_backColor, 1);

                var cellformat = new StringFormat()
                {
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit,
                };

                var initialRowCount = _rowCount;

                var RowBounds = new RectangleF(0, 0, 0, 0);

                // -------------------
                // draw vertical lines
                // draw the rows of the table

                for(int index = initialRowCount; index < _dataGrid.Items.Count; index++)
                {
                    var paymentDetail = _dataGrid.Items[index] as Amortization.PaymentDetail;

                    if(paymentDetail != null)
                    {
                        var startxposition = _location;

                        RowBounds.X = _location;

                        RowBounds.Y = _location + _topMargin +
                                      ((_rowCount - initialRowCount) + 1) * (_font.SizeInPoints + _cellMargin);

                        RowBounds.Width = _pageWidth;
                        RowBounds.Height = _font.SizeInPoints + _cellMargin;

                        _lines.Add(RowBounds.Bottom);

                        graphics.FillRectangle((index % 2 == 0) ? AlternatingBackBrush : BackBrush, RowBounds);

                        foreach(var property in paymentDetail.GetProperties())
                        {
                            var cellbounds = new RectangleF(startxposition,
                                                            _location + _topMargin +
                                                            ((_rowCount - initialRowCount) + 1) * (_font.SizeInPoints + _cellMargin),
                                                            _columnwidth,
                                                            _font.SizeInPoints + _cellMargin);

                            if(startxposition + _columnwidth <= _pageWidth + 25)
                            {
                                graphics.DrawString(property.Value, _font, ForeBrush, cellbounds, cellformat);
                                lastRowBottom = (int)cellbounds.Bottom;
                            }

                            startxposition = startxposition + _columnwidth;
                        }

                        _rowCount++;

                        if(_rowCount * (_font.SizeInPoints + _cellMargin) > (_pPageHeight * _pageNumber) - (_bottomMargin + _topMargin))
                        {
                            DrawHorizontalLines(graphics, _lines);
                            DrawVerticalGridLines(graphics, TheLinePen, _columnwidth, lastRowBottom);
                            return true;
                        }
                    }
                }

                DrawHorizontalLines(graphics, _lines);
                DrawVerticalGridLines(graphics, TheLinePen, _columnwidth, lastRowBottom);

                return false;

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return false;
            }
        }
    }
}
