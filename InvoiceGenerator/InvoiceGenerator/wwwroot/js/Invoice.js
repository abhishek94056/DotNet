let savedInvoiceNo = null;
let currentStateCode = 27;

document.addEventListener('DOMContentLoaded', () => {
    loadCompanyDetails('invoiceTo');
    loadCompanyDetails('shippingTo');
    recalcTotal();
    refreshItemDropdowns(); // ← ADD THIS
});

// ── Load Company Details ──
//async function loadCompanyDetails(selectId) {
//    const name = document.getElementById(selectId).value;
//    if (!name) return;
//    try {
//        const r = await fetch(`/Invoice/GetCompanyDetails?name=${encodeURIComponent(name)}`);
//        const d = await r.json();
//        if (!d) return;
//        if (selectId === 'invoiceTo') {
//            document.getElementById('it_CompanyName').textContent = d.companyName || '';
//            document.getElementById('it_Address').textContent = d.address || '';
//            document.getElementById('it_StateCode').textContent = d.stateCode || '';
//            document.getElementById('it_State').textContent = d.state || '';
//            document.getElementById('it_GSTIN').textContent = d.gstin || '';
//            document.getElementById('it_PaymentTerm').textContent = d.paymentTerm || '—';
//            currentStateCode = parseInt(d.stateCode) || 0;
//            recalcTotal();
//        } else {
//            document.getElementById('st_CompanyName').textContent = d.companyName || '';
//            document.getElementById('st_Address').textContent = d.address || '';
//            document.getElementById('st_StateCode').textContent = d.stateCode || '';
//            document.getElementById('st_State').textContent = d.state || '';
//            document.getElementById('st_GSTIN').textContent = d.gstin || '';
//        }
//    } catch (err) { console.error(err); }
//}
async function loadCompanyDetails(selectId) {

    const name = document.getElementById(selectId).value;
    if (!name) return;

    try {

        const r = await fetch(`/Invoice/GetCompanyDetails?name=${encodeURIComponent(name)}`);
        const d = await r.json();

        if (!d) return;

        if (selectId === 'invoiceTo') {

            document.getElementById('it_CompanyName').textContent = d.CompanyName || '';
            document.getElementById('it_Address').textContent = d.Address || '';
            document.getElementById('it_StateCode').textContent = d.StateCode || '';
            document.getElementById('it_State').textContent = d.State || '';
            document.getElementById('it_GSTIN').textContent = d.GSTIN || '';
            document.getElementById('it_PaymentTerm').textContent = d.PaymentTerm || '—';

            currentStateCode = parseInt(d.StateCode) || 0;

            recalcTotal();

        } else {

            document.getElementById('st_CompanyName').textContent = d.CompanyName || '';
            document.getElementById('st_Address').textContent = d.Address || '';
            document.getElementById('st_StateCode').textContent = d.StateCode || '';
            document.getElementById('st_State').textContent = d.State || '';
            document.getElementById('st_GSTIN').textContent = d.GSTIN || '';
        }

    } catch (err) {

        console.error(err);
    }
}
// ── Item Dropdown Change ──
async function onItemChange(sel) {
    const row = sel.closest('tr');
    if (!sel.value) {
        row.querySelector('.itemCode').textContent = '';
        row.querySelector('.hsn').textContent = '';
        row.querySelector('.rate').textContent = '';
        row.querySelector('.gst').textContent = '';
        row.querySelector('.qty').value = '';
        row.querySelector('.taxableAmt').textContent = '0.00';
        row.querySelector('.gstAmt').textContent = '0.00';
        row.querySelector('.amount').textContent = '0.00';
        recalcTotal();
        return;
    }
    try {
        const r = await fetch(`/Invoice/GetItemDetails?itemName=${encodeURIComponent(sel.value)}`);
        const d = await r.json();
        if (!d) return;

        row.querySelector('.itemCode').textContent = d.ItemCode || '';
        row.querySelector('.hsn').textContent = d.hsnCode || d.HSNCode || '';
        row.querySelector('.rate').textContent = parseFloat(d.rate || d.Rate || 0).toFixed(2);
        row.querySelector('.gst').textContent = parseFloat(d.gst || d.GST || 0).toFixed(0);

        row.querySelector('.qty').value = '';
        row.querySelector('.taxableAmt').textContent = '0.00';
        row.querySelector('.gstAmt').textContent = '0.00';
        row.querySelector('.amount').textContent = '0.00';
        recalcTotal();
    } catch (err) { console.error(err); }
}

// ── Calculate Row ──
function calcRow(qtyInput) {
    const row = qtyInput.closest('tr');
    const rate = parseFloat(row.querySelector('.rate').textContent) || 0;
    const qty = parseFloat(qtyInput.value) || 0;
    const gstPct = parseFloat(row.querySelector('.gst').textContent) || 0;

    const taxable = rate * qty;
    const gstAmt = taxable * (gstPct / 100);
    const total = taxable + gstAmt;

    row.querySelector('.taxableAmt').textContent = taxable.toFixed(2);
    row.querySelector('.gstAmt').textContent = gstAmt.toFixed(2);
    row.querySelector('.amount').textContent = total.toFixed(2);

    recalcTotal();
}

// ── Recalculate Totals ──
function recalcTotal() {
    let sumTaxable = 0, sumGst = 0, sumTotal = 0;

    document.querySelectorAll('#itemsBody tr').forEach(row => {
        sumTaxable += parseFloat(row.querySelector('.taxableAmt')?.textContent) || 0;
        sumGst += parseFloat(row.querySelector('.gstAmt')?.textContent) || 0;
        sumTotal += parseFloat(row.querySelector('.amount')?.textContent) || 0;
    });

    // Footer row totals
    document.getElementById('footTaxable').textContent = sumTaxable.toFixed(2);
    document.getElementById('footGst').textContent = sumGst.toFixed(2);
    document.getElementById('footTotal').textContent = sumTotal.toFixed(2);

    // Summary box
    document.getElementById('taxableValue').textContent = sumTaxable.toFixed(2);

    let cgst = 0, sgst = 0, igst = 0;
    if (currentStateCode === 27) {
        cgst = sumGst / 2;
        sgst = sumGst / 2;
    } else {
        igst = sumGst;
    }

    document.getElementById('cgst').textContent = cgst.toFixed(2);
    document.getElementById('sgst').textContent = sgst.toFixed(2);
    document.getElementById('igst').textContent = igst.toFixed(2);
    document.getElementById('totalValue').textContent = sumTotal.toFixed(2);

    document.getElementById('totalWords').textContent =
        sumTotal > 0 ? convertToWords(sumTotal) : '';
}

// ── Add Row ──
// function addRow() {
//     const tbody   = document.getElementById('itemsBody');
//     const count   = tbody.rows.length + 1;
//     const options = document.getElementById('itemOptionsTemplate').innerHTML;
//     tbody.insertAdjacentHTML('beforeend', `
//         <tr>
//             <td class="text-center align-middle">${count}</td>
//             <td class="itemCode text-center align-middle"></td>
//             <td>
//                 <select class="form-select form-select-sm item-select"
//                         onchange="onItemChange(this)">
//                     ${options}
//                 </select>
//             </td>
//             <td class="hsn text-center align-middle"></td>
//             <td class="rate text-center align-middle"></td>
//             <td>
//                 <input type="number" class="form-control form-control-sm qty"
//                        oninput="calcRow(this)" min="0" step="any"/>
//             </td>
//             <td class="gst text-center align-middle"></td>
//             <td class="taxableAmt text-end align-middle pe-2">0.00</td>
//             <td class="gstAmt text-end align-middle pe-2">0.00</td>
//             <td class="amount text-end align-middle pe-2 fw-bold">0.00</td>
//             <td class="text-center align-middle">
//                 <button class="btn btn-sm btn-outline-danger py-0 px-1"
//                         onclick="removeRow(this)">✕</button>
//             </td>
//         </tr>`);
// }
function addRow() {
    const tbody = document.getElementById('itemsBody');
    const count = tbody.rows.length + 1;
    const options = document.getElementById('itemOptionsTemplate').innerHTML;

    tbody.insertAdjacentHTML('beforeend', `
                <tr>
                    <td class="text-center align-middle">${count}</td>
                    <td class="itemCode text-center align-middle"></td>

                    <td>
                        <select class="form-select form-select-sm item-select"
                                onchange="onItemChange(this)">
                            ${options}
                        </select>
                    </td>

                    <td class="hsn text-center align-middle"></td>
                    <td class="rate text-center align-middle"></td>

                    <!-- ✅ FIXED QTY -->
                    <td class="text-center align-middle">
                        <div class="d-flex justify-content-center">
                            <input type="number"
                                   class="form-control form-control-sm qty text-center"
                                   style="width:70px;"
                                   oninput="calcRow(this)" min="0" step="any"/>
                        </div>
                    </td>

                    <td class="gst text-center align-middle"></td>
                    <td class="taxableAmt text-end align-middle pe-2">0.00</td>
                    <td class="gstAmt text-end align-middle pe-2">0.00</td>
                    <td class="amount text-end align-middle pe-2 fw-bold">0.00</td>

                    <!-- ✅ SAME ICON + ALIGN -->
                    <td class="text-center align-middle">
                        <button class="btn btn-sm btn-outline-danger py-0 px-1"
                                onclick="removeRow(this)">×</button>
                    </td>
                </tr>
            `);

    refreshItemDropdowns();
}
// ── Remove Row ──
// function removeRow(btn) {
//     const tbody = document.getElementById('itemsBody');
//     if (tbody.rows.length === 1) {
//         alert('At least one item row is required.');
//         return;
//     }
//     btn.closest('tr').remove();
//     Array.from(tbody.rows).forEach((row, i) => {
//         row.cells[0].textContent = i + 1;
//     });
//     recalcTotal();
// }
function removeRow(btn) {
    const tbody = document.getElementById('itemsBody');
    if (tbody.rows.length === 1) {
        alert('At least one item row is required.');
        return;
    }
    btn.closest('tr').remove();

    // Re-number Sr column
    Array.from(tbody.rows).forEach((row, i) => {
        row.cells[0].textContent = i + 1;
    });

    recalcTotal();
    refreshItemDropdowns(); // ← ADD THIS — frees the removed item's option
}
// ── Save Invoice ──
async function saveInvoice() {
    if (!document.getElementById('invoiceDate').value) { alert('Please select Invoice Date.'); return; }
    if (!document.getElementById('dateOfSupply').value) { alert('Please select Date of Supply.'); return; }
    if (!document.getElementById('poDate').value) { alert('Please select PO Date.'); return; }
    if (!document.getElementById('invoiceTo').value) { alert('Please select Invoice To.'); return; }
    if (!document.getElementById('shippingTo').value) { alert('Please select Shipping To.'); return; }

    const items = [];
    let hasError = false;

    document.querySelectorAll('#itemsBody tr').forEach((row, i) => {
        const desc = row.querySelector('.item-select')?.value;
        const qty = parseFloat(row.querySelector('.qty').value) || 0;
        if (!desc || qty <= 0) { hasError = true; return; }

        items.push({
            srNo: i + 1,
            itemCode: row.querySelector('.itemCode').textContent.trim(),
            itemDescription: desc,
            hsnCode: row.querySelector('.hsn').textContent.trim(),
            rate: parseFloat(row.querySelector('.rate').textContent) || 0,
            qty: qty,
            gst: parseFloat(row.querySelector('.gst').textContent) || 0,
            taxableAmount: parseFloat(row.querySelector('.taxableAmt').textContent) || 0,
            gstAmount: parseFloat(row.querySelector('.gstAmt').textContent) || 0,
            amount: parseFloat(row.querySelector('.amount').textContent) || 0
        });
    });

    if (hasError || items.length === 0) {
        alert('Please fill all item rows completely (Item + Qty).');
        return;
    }

    const payload = {
        master: {
            invoiceDate: document.getElementById('invoiceDate').value,
            dateOfSupply: document.getElementById('dateOfSupply').value,
            purchaseOrderNo: document.getElementById('poNo').value.trim(),
            purchaseOrderDt: document.getElementById('poDate').value,
            vehicleNo: document.getElementById('vehicleNo').value.trim(),
            aSNNo: document.getElementById('ASNNo').value.trim(),
            invoiceTo: document.getElementById('invoiceTo').value,
            shippingTo: document.getElementById('shippingTo').value,
            taxableValue: parseFloat(document.getElementById('taxableValue').textContent) || 0,
            transportMode: document.getElementById('transportMode').value,
            remark: document.getElementById('remark').value.trim()
        },
        items
    };

    try {
        const r = await fetch('/Invoice/Save', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });
        if (!r.ok) throw new Error(`Server error: ${r.status}`);
        const d = await r.json();
        if (d.success) {
            savedInvoiceNo = d.invoiceNo;
            alert(`✅ Saved! Invoice No: FY2526/CNT/${d.invoiceNo}`);
            document.getElementById('btnPdf').style.display = 'inline-block';
        } else {
            alert('❌ Save failed: ' + (d.message || 'Unknown error'));
        }
    } catch (err) {
        alert('❌ Error: ' + err.message);
    }
}

// ── Download PDF ──
function downloadPdf() {
    if (!savedInvoiceNo) { alert('Please save the invoice first.'); return; }
    window.open(`/Invoice/Pdf/${savedInvoiceNo}`, '_blank');
}

// ── Convert to Words ──
function convertToWords(amount) {
    const ones = ['', 'ONE', 'TWO', 'THREE', 'FOUR', 'FIVE', 'SIX', 'SEVEN', 'EIGHT', 'NINE',
        'TEN', 'ELEVEN', 'TWELVE', 'THIRTEEN', 'FOURTEEN', 'FIFTEEN', 'SIXTEEN',
        'SEVENTEEN', 'EIGHTEEN', 'NINETEEN'];
    const tens = ['', '', 'TWENTY', 'THIRTY', 'FORTY', 'FIFTY',
        'SIXTY', 'SEVENTY', 'EIGHTY', 'NINETY'];
    function n2w(n) {
        n = Math.floor(n);
        if (n === 0) return '';
        if (n < 20) return ones[n] + ' ';
        if (n < 100) return tens[Math.floor(n / 10)] + ' ' + ones[n % 10] + ' ';
        if (n < 1000) return ones[Math.floor(n / 100)] + ' HUNDRED ' + n2w(n % 100);
        if (n < 100000) return n2w(Math.floor(n / 1000)) + 'THOUSAND ' + n2w(n % 1000);
        if (n < 10000000) return n2w(Math.floor(n / 100000)) + 'LAKH ' + n2w(n % 100000);
        return n2w(Math.floor(n / 10000000)) + 'CRORE ' + n2w(n % 10000000);
    }
    const rupees = Math.floor(amount);
    const paise = Math.round((amount - rupees) * 100);
    let result = 'RUPEES ' + n2w(rupees).trim();
    if (paise > 0) result += ' AND ' + n2w(paise).trim() + ' PAISE';
    return result + ' ONLY';
}
// ════════════════════════════════════════════
// PREVENT DUPLICATE ITEM SELECTION IN ROWS
// ════════════════════════════════════════════

// Call this whenever any dropdown changes or a row is added/removed
function refreshItemDropdowns() {
    const allSelects = Array.from(
        document.querySelectorAll('#itemsBody .item-select'));

    // Collect all currently selected values (excluding empty)
    const selectedValues = allSelects
        .map(sel => sel.value)
        .filter(v => v !== '' && v !== null);

    // For each dropdown — disable options selected in OTHER rows
    allSelects.forEach(sel => {
        const thisValue = sel.value;

        Array.from(sel.options).forEach(opt => {
            if (opt.value === '' || opt.value === thisValue) {
                // Always keep blank + own selection enabled
                opt.disabled = false;
                opt.style.color = '';
                opt.title = '';
            } else if (selectedValues.includes(opt.value)) {
                // Disable if selected in another row
                opt.disabled = true;
                opt.style.color = '#aaa';
                opt.title = 'Already selected in another row';
            } else {
                opt.disabled = false;
                opt.style.color = '';
                opt.title = '';
            }
        });
    });
}