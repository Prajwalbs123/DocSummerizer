// wwwroot/js/dragDrop.js
export function startFileDropZone(dropzoneElement /*div element where drop happens*/, inputFile) {

    function onDragHover(e){
        e.preventDefault();
        dropzoneElement.classList.add("hover");
    }

    function onDragLeave(e) {
        e.preventDefault();
        dropzoneElement.classList.remove("hover");
    }

    function onDrop(e) {
        e.preventDefault();
        dropzoneElement.classList.remove("hover");
        inputFile.files = e.dataTransfer.files;
        const event = new Event('change', { bubbles: true });
        inputFile.dispatchEvent(event);
    }

    function onPaste(e) {
        inputFile.files = e.clipboardData.files;
        const event = new Event('change', { bubbles: true });
        inputFile.dispatchEvent(event);
    }

    dropzoneElement.addEventListener("dragenter", onDragHover);
    dropzoneElement.addEventListener("dragover", onDragHover);
    dropzoneElement.addEventListener("dragleave", onDragLeave);
    dropzoneElement.addEventListener("drop", onDrop);
    dropzoneElement.addEventListener("paste", onPaste);



    return {
        dispose: () => {
            dropzoneElement.removeEventListener("dragenter", onDragHover);
            dropzoneElement.removeEventListener("dragover", onDragHover);
            dropzoneElement.removeEventListener("dragleave", onDragLeave);
            dropzoneElement.removeEventListener("drop", onDrop);
            dropzoneElement.removeEventListener("paste", onPaste);
        }
    }
}