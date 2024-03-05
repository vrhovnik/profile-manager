// Usage: Add class="sortable" to the table tag and class="sort" to the th tags
document.addEventListener('click', function (e) {
    try {
        function findElementRecursive(element, tag) {
            return element.nodeName === tag ? element :
                findElementRecursive(element.parentNode, tag)
        }

        const descending_th_class = ' dir-d ';
        const ascending_th_class = ' dir-u ';
        const ascending_table_sort_class = 'asc';
        const regex_dir = / dir-(u|d) /;
        const regex_table = /\bsortable\b/;
        const alt_sort = e.shiftKey || e.altKey;
        let element = findElementRecursive(e.target, 'TH');
        const tr = findElementRecursive(element, 'TR');
        const table = findElementRecursive(tr, 'TABLE');

        function reClassify(element, dir) {
            element.className = element.className.replace(regex_dir, '') + dir
        }
        function getValue(element) {
            return (
                (alt_sort && element.getAttribute('data-sort-alt')) ||
                element.getAttribute('data-sort') || element.innerText
            )
        }
        if (regex_table.test(table.className)) {
            let column_index;
            const nodes = tr.cells;
            for (let i = 0; i < nodes.length; i++) {
                if (nodes[i] === element) {
                    column_index = element.getAttribute('data-sort-col') || i
                } else {
                    reClassify(nodes[i], '')
                }
            }
            let dir = descending_th_class;
            if (
                element.className.indexOf(descending_th_class) !== -1 ||
                (table.className.indexOf(ascending_table_sort_class) !== -1 &&
                    element.className.indexOf(ascending_th_class) === -1)
            ) {
                dir = ascending_th_class
            }
            reClassify(element, dir)
            const org_tbody = table.tBodies[0];
            const rows = [].slice.call(org_tbody.rows, 0);
            const reverse = dir === ascending_th_class;
            rows.sort(function (a, b) {
                const x = getValue((reverse ? a : b).cells[column_index]);
                const y = getValue((reverse ? b : a).cells[column_index]);
                return isNaN(x - y) ? x.localeCompare(y) : x - y
            })
            const clone_tbody = org_tbody.cloneNode();
            while (rows.length) {
                clone_tbody.appendChild(rows.splice(0, 1)[0])
            }
            table.replaceChild(clone_tbody, org_tbody)
        }
    } catch (error) {
    }
});
