    // ════════════════════════════════════════════
    // PAGINATION STATE
    // ════════════════════════════════════════════
    let currentPage   = 1;
    let rowsPerPage   = 15;
    let totalPages    = 1;
    let filteredRows  = [];   // currently visible rows after search

    const modal       = new bootstrap.Modal(document.getElementById('companyModal'));
    const deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));
    const toastEl     = document.getElementById('toast');
    const bsToast     = new bootstrap.Toast(toastEl, { delay: 4000 });
    let   pendingDeleteId = null;

    // ════════════════════════════════════════════
    // PAGINATION FUNCTIONS
    // ════════════════════════════════════════════
    function getAllRows() {
        return Array.from(document.querySelectorAll('#tableBody tr'));
    }

    function filterTable() {
        const q = document.getElementById('searchInput').value.toLowerCase().trim();
        const allRows = getAllRows();

        filteredRows = allRows.filter(row => {
            const text = row.textContent.toLowerCase();
            const match = !q || text.includes(q);
            return match;
        });

        currentPage = 1;
        renderPage();
    }

    function renderPage() {
        const allRows = getAllRows();

        // Hide all rows first
        allRows.forEach(row => row.style.display = 'none');

        totalPages = Math.max(1, Math.ceil(filteredRows.length / rowsPerPage));
        if (currentPage > totalPages) currentPage = totalPages;

        const start = (currentPage - 1) * rowsPerPage;
        const end   = Math.min(start + rowsPerPage, filteredRows.length);

        // Show only current page rows
        for (let i = start; i < end; i++) {
            filteredRows[i].style.display = '';
        }

        

        updatePaginationUI(start, end);
    }

    function updatePaginationUI(start, end) {
        // Page info text
        document.getElementById('pageInfo').textContent =
            filteredRows.length === 0
                ? 'No records'
                : `${start + 1}–${end} of ${filteredRows.length}`;

        // First / Prev / Next / Last buttons
        document.getElementById('btnFirst').disabled = currentPage === 1;
        document.getElementById('btnPrev').disabled  = currentPage === 1;
        document.getElementById('btnNext').disabled  = currentPage === totalPages;
        document.getElementById('btnLast').disabled  = currentPage === totalPages;

        // Page number buttons (max 5 visible)
        const container   = document.getElementById('pageNumbers');
        container.innerHTML = '';

        const maxVisible  = 3;
        let   startPage   = Math.max(1, currentPage - Math.floor(maxVisible / 2));
        let   endPage     = Math.min(totalPages, startPage + maxVisible - 1);

        // Shift window if near end
        if (endPage - startPage < maxVisible - 1)
            startPage = Math.max(1, endPage - maxVisible + 1);

        for (let p = startPage; p <= endPage; p++) {
            const btn = document.createElement('button');
            btn.textContent = p;
            btn.className   = 'btn btn-outline-secondary btn-sm'
                            + (p === currentPage ? ' active-page' : '');
            btn.onclick     = () => goToPage(p);
            container.appendChild(btn);
        }
    }

    function goToPage(page) {
        if (page < 1 || page > totalPages) return;
        currentPage = page;
        renderPage();
    }

    function changeRowsPerPage() {
        rowsPerPage = parseInt(
            document.getElementById('rowsPerPageSelect').value);
        currentPage = 1;
        renderPage();
    }

    // ── Init on load ──
    document.addEventListener('DOMContentLoaded', () => {
        filteredRows = getAllRows();
        renderPage();
    });

    // ════════════════════════════════════════════
    // STATE → STATE CODE AUTO-FILL
    // ════════════════════════════════════════════
    const stateCodes = {
        "Andhra Pradesh": 37, "Arunachal Pradesh": 12, "Assam": 18,
        "Bihar": 10, "Chhattisgarh": 22, "Goa": 30, "Gujarat": 24,
        "Haryana": 6, "Himachal Pradesh": 2, "Jharkhand": 20,
        "Karnataka": 29, "Kerala": 32, "Madhya Pradesh": 23,
        "Maharashtra": 27, "Manipur": 14, "Meghalaya": 17,
        "Mizoram": 15, "Nagaland": 13, "Odisha": 21, "Punjab": 3,
        "Rajasthan": 8, "Sikkim": 11, "Tamil Nadu": 33,
        "Telangana": 36, "Tripura": 16, "Uttar Pradesh": 9,
        "Uttarakhand": 5, "West Bengal": 19, "Delhi": 7,
        "Jammu and Kashmir": 1, "Ladakh": 38, "Chandigarh": 4,
        "Puducherry": 34, "Lakshadweep": 31,
        "Andaman and Nicobar Islands": 35,
        "Dadra and Nagar Haveli and Daman and Diu": 26
    };

    document.getElementById('state').addEventListener('change', function () {
        document.getElementById('stateCode').value =
            stateCodes[this.value] ?? '';
    });

    // ════════════════════════════════════════════
    // CRUD — OPEN MODAL
    // ════════════════════════════════════════════
    function openModal() {
        clearForm();
        document.getElementById('modalTitleText').textContent = 'Add Company';
        modal.show();
    }

    // ── Edit ──
    async function editCompany(id) {
        try {
            const r = await fetch(`/Company/GetById?id=${id}`);
            const d = await r.json();
            document.getElementById('modalTitleText').textContent = 'Edit Company';
            document.getElementById('companyId').value   = d.CompanyId;
            document.getElementById('companyName').value = d.CompanyName;
            document.getElementById('address').value     = d.Address;
            document.getElementById('stateCode').value   = d.StateCode;
            document.getElementById('state').value       = d.State;
            document.getElementById('gstin').value       = d.GSTIN;
            document.getElementById('paymentTerm').value = d.PaymentTerm;
            clearErrors();
            modal.show();
        } catch (err) {
            showToast('Failed to load company data.', 'danger');
        }
    }

    // ── Save ──
    async function saveCompany() {
        clearErrors();

        const companyName = document.getElementById('companyName').value.trim();
        const gstin       = document.getElementById('gstin').value.trim().toUpperCase();
        const address     = document.getElementById('address').value.trim();
        const stateCode   = document.getElementById('stateCode').value.trim();
        const state       = document.getElementById('state').value;
        const paymentTerm = document.getElementById('paymentTerm').value;

        let valid = true;

        if (!companyName) {
            showError('companyName', 'err-companyName', 'Company Name is required.');
            valid = false;
        }
        if (!gstin) {
            showError('gstin', 'err-gstin', 'GSTIN is required.');
            valid = false;
        } else if (gstin.length !== 15) {
            showError('gstin', 'err-gstin', 'GSTIN must be exactly 15 characters.');
            valid = false;
        }
        if (!address) {
            showError('address', 'err-address', 'Address is required.');
            valid = false;
        }
        if (!state) {
            showError('state', 'err-state', 'Please select a State.');
            valid = false;
        }
        if (!stateCode || parseInt(stateCode) < 1 || parseInt(stateCode) > 99) {
            showError('stateCode', 'err-stateCode', 'Valid State Code required.');
            valid = false;
        }
        if (!paymentTerm) {
            showError('paymentTerm', 'err-paymentTerm', 'Please select a Payment Term.');
            valid = false;
        }

        if (!valid) return;

        const token = document.querySelector(
            '#companyForm input[name="__RequestVerificationToken"]').value;

        const payload = new URLSearchParams({
            CompanyId:   document.getElementById('companyId').value,
            CompanyName: companyName,
            Address:     address,
            StateCode:   stateCode,
            State:       state,
            GSTIN:       gstin,
            PaymentTerm: paymentTerm,
            __RequestVerificationToken: token
        });

        try {
            const r = await fetch('/Company/Save', {
                method:  'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body:    payload
            });
            const d = await r.json();

            if (!d.success) {
                if (d.errors) d.errors.forEach(e => showToast(e, 'danger'));
                else showToast(d.message || 'Save failed.', 'danger');
                return;
            }

            modal.hide();
            showToast(d.message || 'Company saved successfully!', 'success');
            setTimeout(() => location.reload(), 1200);
        } catch (err) {
            showToast('Network error. Please try again.', 'danger');
        }
    }


    // ── Delete confirm ──
    function deleteCompany(id, name) {
        pendingDeleteId = id;
        document.getElementById('deleteCompanyName').textContent = name;
        deleteModal.show();
    }

    document.getElementById('btnConfirmDelete').addEventListener('click', async () => {
        const token = document.querySelector(
            'input[name="__RequestVerificationToken"]').value;
        try {
            const r = await fetch('/Company/Delete', {
                method:  'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body:    new URLSearchParams({
                    id: pendingDeleteId,
                    __RequestVerificationToken: token
                })
            });
            const d = await r.json();
            deleteModal.hide();
            if (d.success) {
                document.getElementById(`row-${pendingDeleteId}`)?.remove();
                filteredRows = filteredRows.filter(
                    row => row.id !== `row-${pendingDeleteId}`);
                renderPage();
                showToast('Company deleted successfully.', 'success');
            } else {
                showToast('Delete failed.', 'danger');
            }
        } catch (err) {
            showToast('Network error.', 'danger');
        }
    });

    // ════════════════════════════════════════════
    // HELPERS
    // ════════════════════════════════════════════
    function clearForm() {
        ['companyId','companyName','address',
         'stateCode','gstin'].forEach(id =>
            document.getElementById(id).value = id === 'companyId' ? '0' : '');
        document.getElementById('state').value       = '';
        document.getElementById('paymentTerm').value = '';
        clearErrors();
    }

    function clearErrors() {
        document.querySelectorAll('.invalid-feedback')
            .forEach(el => { el.textContent = ''; });
        document.querySelectorAll('.form-control, .form-select')
            .forEach(el => el.classList.remove('is-invalid'));
    }

    function showError(fieldId, errId, msg) {
        document.getElementById(fieldId)?.classList.add('is-invalid');
        document.getElementById(errId).textContent = msg;
    }

    function showToast(msg, type = 'success') {
        toastEl.className =
            `toast align-items-center text-white border-0 bg-${type}`;
        document.getElementById('toastMsg').textContent = msg;
        bsToast.show();
    }