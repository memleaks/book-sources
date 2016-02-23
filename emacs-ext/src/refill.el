(require 'limited)

(defun refill (start end len)
  "After a text change, refill the current paragraph."
  (let ((left (if (or (zerop len)
                      (not (before-2nd-word-p start)))
                  start
                (limited-save-excursion
                  (max (progn
			 (goto-char start)
			 (beginning-of-line 0)
			 (point))
		       (progn
			 (goto-char start)
			 (backward-paragraph 1)
			 (point)))))))
    (if (or (and (zerop len)
                 (same-line-p start end)
                 (short-line-p end))
            (and (eq (char-syntax (preceding-char))
                     ?\ )
                 (looking-at "\\s *$")))
        nil
      (limited-save-excursion
        (fill-region left end nil nil t)))))

(defun short-line-p (pos)
  "Does line containing POS stay within `fill-column'?"
  (limited-save-excursion
    (goto-char pos)
    (end-of-line)
    (<= (current-column) fill-column)))

(defun same-line-p (start end)
  "Are START and END on the same line?"
  (limited-save-excursion
    (goto-char start)
    (end-of-line)
    (<= end (point))))

(defun before-2nd-word-p (pos)
  "Does POS lie before the second word on the line?"
  (limited-save-excursion
    (goto-char pos)
    (beginning-of-line)
    (skip-syntax-forward (concat "^ "
                                 (char-to-string
                                  (char-syntax ?\n))))
    (skip-syntax-forward " ")
    (< pos (point))))

(defvar refill-mode nil
  "Mode variable for refill minor mode.")
(make-variable-buffer-local 'refill-mode)

(if (not (assq 'refill-mode minor-mode-alist))
    (setq minor-mode-alist
          (cons '(refill-mode " Refill")
                minor-mode-alist)))

(defun refill-mode (&optional arg)
  "Refill minor mode."
  (interactive "P")
  (setq refill-mode
        (if (null arg)
            (not refill-mode)
          (> (prefix-numeric-value arg) 0)))
  (make-local-hook 'after-change-functions)
  (if refill-mode
      (add-hook 'after-change-functions 'refill nil t)
    (remove-hook 'after-change-functions 'refill t)))

(provide 'refill)
