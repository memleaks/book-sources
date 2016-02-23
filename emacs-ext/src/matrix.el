(defun make-matrix (rows columns &optional initial)
  "Create a ROWS by COLUMNS matrix."
  (let ((result (make-vector rows nil))
        (y 0))
    (while (< y rows)
      (aset result y (make-vector columns initial))
      (setq y (+ y 1)))
    result))

(defun matrix-set (matrix row column elt)
  "Given a MATRIX, ROW, and COLUMN, put element ELT there."
  (let ((nested-vector (aref matrix row)))
     (aset nested-vector column elt)))

(defun matrix-ref (matrix row column)
  "Get the element of MATRIX at ROW and COLUMN."
  (let ((nested-vector (aref matrix row)))
    (aref nested-vector column)))

(defun matrix-columns (matrix)
  "Number of columns in MATRIX."
  (length (aref matrix 0)))

(defun matrix-rows (matrix)
  "Number of rows in MATRIX."
  (length matrix))

(provide 'matrix)
