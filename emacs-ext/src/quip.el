(require 'derived)

(define-derived-mode quip-mode text-mode "Quip"
  "Major mode for editing Quip files.
Special commands:
\\{quip-mode-map}"
  (make-local-variable 'paragraph-separate)
  (make-local-variable 'paragraph-start)
  (make-local-variable 'page-delimiter)
  (setq paragraph-start "%%\\|[ \t\n\^L]")
  (setq paragraph-separate "%%$\\|[ \t\^L]*$")
  (setq page-delimiter "^%%$"))

(define-key quip-mode-map "\C-x[" 'backward-quip)
(define-key quip-mode-map "\C-x]" 'forward-quip)
(define-key quip-mode-map "\C-xnq" 'narrow-to-quip)
(define-key quip-mode-map "\C-cw" 'what-quip)

(defun count-quips ()
  "Count the quips in the buffer."
  (interactive)
  (save-excursion
    (save-restriction
      (widen)
      (goto-char (point-min))
      (count-matches "^%%$"))))

(defalias 'backward-quip 'backward-page)
(defalias 'forward-quip 'forward-page)
(defalias 'narrow-to-quip 'narrow-to-page)
(defalias 'what-quip 'what-page)

(provide 'quip)
