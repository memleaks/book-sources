(require 'matrix)

(defun make-crossword (size)
  "Make a crossword grid with SIZE rows and columns."
  (if (zerop (% size 2))
      (error "make-crossword: size must be odd"))
  (if (< size 3)
      (error "make-crossword: size must be 3 or greater"))
  (make-matrix size size nil))

(defsubst crossword-size (crossword)
  "Number of rows and columns in CROSSWORD."
  (matrix-rows crossword))

(defsubst crossword-ref (crossword row column)
  "Get the element of CROSSWORD at ROW and COLUMN."
  (matrix-ref crossword row column))

(defsubst crossword--set (crossword row column elt)
  "Internal function for setting a crossword grid square."
  (matrix-set crossword row column elt))

(defun crossword-cousin-position (crossword row column)
  "Give the cousin position for CROSSWORD ROW and COLUMN."
  (let ((size (crossword-size crossword)))
    (cons (- size row 1) (- size column 1))))

(defun crossword-cousin-ref (crossword row column)
  "Get the cousin of CROSSWORD's ROW,COLUMN position."
  (let ((cousin-position (crossword-cousin-position crossword
                                                    row
                                                    column)))
    (crossword-ref crossword
                   (car cousin-position)
                   (cdr cousin-position))))

(defun crossword--cousin-set (crossword row column elt)
  "Internal function for setting the cousin of a cell."
  (let ((cousin-position (crossword-cousin-position crossword
                                                    row
                                                    column)))
    (crossword--set crossword
                    (car cousin-position)
                    (cdr cousin-position)
                    elt)))

(defun crossword-store-letter (crossword row column letter)
  "Given CROSSWORD, ROW, and COLUMN, put LETTER there."
  (crossword--set crossword row column letter)
  (if (numberp (crossword-cousin-ref crossword row column))
      nil
    (crossword--cousin-set crossword row column 'letter)))

(defun crossword-store-block (crossword row column)
  "Given CROSSWORD, ROW, and COLUMN, put a block there."
  (crossword--set crossword row column 'block)
  (crossword--cousin-set crossword row column 'block))

(defun crossword-clear-cell (crossword row column)
  "Erase the CROSSWORD cell at ROW,COLUMN."
  (let ((cousin-position (crossword-cousin-position crossword
                                                    row
                                                    column)))
    (if (and (not (equal cousin-position
                         (cons row column)))
             (numberp (crossword-ref crossword
                                     (car cousin-position)
                                     (cdr cousin-position))))
        (crossword--set crossword row column 'letter)
      (crossword--set crossword row column nil)
      (crossword--set crossword
                      (car cousin-position)
                      (cdr cousin-position)
                      nil))))

(defun crossword-one-letter-p (crossword row column)
  "Is CROSSWORD cell at ROW,COLUMN a one-letter word?"
  (and (not (eq (crossword-ref crossword row column)
                'block))
       (or (and (crossword-block-p crossword (- row 1) column)
                (crossword-block-p crossword (+ row 1) column))
           (and (crossword-block-p crossword row (- column 1))
                (crossword-block-p crossword row (+ column 1))))))

(defun crossword-block-p (crossword row column)
  "Does CROSSWORD's ROW,COLUMN cell contain a block?"
  (or (< row 0)
      (>= row (crossword-size crossword))
      (< column 0)
      (>= column (crossword-size crossword))
      (eq (crossword-ref crossword row column) 'block)))

(defun crossword-insert-row (row)
  "Insert ROW into the current buffer."
  (mapcar 'crossword-insert-cell row)
  (insert "\n"))

(defun crossword-insert-cell (cell)
  "Insert CELL into the current buffer."
  (insert (cond ((null cell) ".")
                ((eq cell 'letter) "?")
                ((eq cell 'block) "#")
                ((numberp cell) cell))
          " "))

(defun crossword-place-cursor (row column)
  "Move point to ROW,COLUMN."
  (goto-char (point-min))
  (forward-line row)
  (forward-char (* column 2)))

(defun crossword-cursor-coords ()
  "Compute (ROW . COLUMN) from cursor position."
  (cons (- (current-line) 1)
        (/ (current-column) 2)))

(defun current-line ()
  "Return line number containing point."
  (let ((result 1))
    (save-excursion
      (beginning-of-line)
      (while (not (bobp))
        (forward-line -1)
        (setq result (+ result 1))))
    result))

(defun crossword-erase-command ()
  "Erase current crossword cell."
  (interactive)
  (let ((coords (crossword-cursor-coords)))
    (crossword-clear-cell crossword-grid
                          (car coords)
                          (cdr coords)))
  (crossword-update-display crossword-grid))

(defun crossword-block-command ()
  "Insert a block in current cell and cousin."
  (interactive)
  (let ((coords (crossword-cursor-coords)))
    (crossword-store-block crossword-grid
                           (car coords)
                           (cdr coords)))
  (crossword-update-display crossword-grid))

(defun crossword-self-insert ()
  "Self-insert letter in current cell."
  (interactive)
  (let ((coords (crossword-cursor-coords)))
    (crossword-store-letter crossword-grid
                            (car coords)
                            (cdr coords)
                            (aref (this-command-keys) 0)))
  (crossword-update-display crossword-grid))

(defun crossword-cursor-right (arg)
  "Move ARG cells to the right."
  (interactive "p")
  (let* ((coords (crossword-cursor-coords))
         (new-column (+ arg (cdr coords))))
    (if (or (< new-column 0)
            (>= new-column (crossword-size crossword-grid)))
        (error "Out of bounds"))
    (crossword-place-cursor (car coords)
                            new-column)))

(defun crossword-cursor-left (arg)
  "Move ARG cells to the left."
  (interactive "p")
  (crossword-cursor-right (- arg)))

(defun crossword-cursor-down (arg)
  "Move ARG cells down."
  (interactive "p")
  (let* ((coords (crossword-cursor-coords))
         (new-row (+ arg (car coords))))
    (if (or (< new-row 0)
            (>= new-row (crossword-size crossword-grid)))
        (error "Out of bounds"))
    (crossword-place-cursor new-row
                            (cdr coords))))

(defun crossword-cursor-up (arg)
  "Move ARG cells up."
  (interactive "p")
  (crossword-cursor-down (- arg)))

(defun crossword-beginning-of-row ()
  "Move to beginning of current row."
  (interactive)
  (let ((coords (crossword-cursor-coords)))
    (crossword-place-cursor (car coords) 0)))

(defun crossword-end-of-row ()
  "Move to end of current row."
  (interactive)
  (let ((coords (crossword-cursor-coords)))
    (crossword-place-cursor (car coords)
                            (- (crossword-size crossword-grid)
                               1))))

(defun crossword-top-of-column ()
  "Move to top of current column."
  (interactive)
  (let ((coords (crossword-cursor-coords)))
    (crossword-place-cursor 0 (cdr coords))))

(defun crossword-bottom-of-column ()
  "Move to bottom of current row."
  (interactive)
  (let ((coords (crossword-cursor-coords)))
    (crossword-place-cursor (- (crossword-size crossword-grid)
                               1)
                            (cdr coords))))

(defun crossword-beginning-of-grid ()
  "Move to beginning of grid."
  (interactive)
  (crossword-place-cursor 0 0))

(defun crossword-end-of-grid ()
  "Move to end of grid."
  (interactive)
  (let ((size (crossword-size crossword-grid)))
    (crossword-place-cursor size size)))

(defun crossword-jump-to-cousin ()
  "Move to cousin of current cell."
  (interactive)
  (let* ((coords (crossword-cursor-coords))
         (cousin (crossword-cousin-position crossword-grid
                                            (car coords)
                                            (cdr coords))))
    (crossword-place-cursor (car cousin)
                            (cdr cousin))))

(defun crossword--mode-setup (grid)
  "Auxiliary function to set up crossword mode."
  (kill-all-local-variables)
  (setq major-mode 'crossword-mode)
  (setq mode-name "Crossword")
  (use-local-map crossword-mode-map)
  (make-local-variable 'crossword-grid)
  (make-local-hook 'after-change-functions)
  (add-hook 'after-change-functions
	    'crossword-after-change-function)
  (make-local-hook 'post-command-hook)
  (add-hook 'post-command-hook
	    'crossword-post-command-function)
  (setq crossword-grid grid)
  (crossword-place-cursor 0 0)
  (run-hooks 'crossword-mode-hook))

(defun crossword (size)
  "Create a new buffer with an empty crossword grid."
  (interactive "nGrid size: ")
  (let* ((grid (make-crossword size))
         (buffer (generate-new-buffer "*Crossword*")))
    (switch-to-buffer buffer)
    (crossword-insert-grid grid)
    (crossword--mode-setup grid)))

(defvar crossword-egrep-program "egrep"
  "*Egrep program for crossword-hwords and crossword-vwords.")
(defvar crossword-words-file "/usr/local/share/dict/words"
  "*Words file for crossword-hwords and crossword-vwords.")

(defun crossword-mode ()
  "Major mode for editing crossword puzzles.
Special commands:
\\{crossword-mode-map}"
  (interactive)
  (crossword--mode-setup (crossword-parse-buffer)))

(defvar crossword-menu-map nil
  "Menu for Crossword mode.")

(if crossword-menu-map
    nil
  (setq crossword-menu-map (make-sparse-keymap "Crossword"))
  (define-key crossword-menu-map [find-singleton]
              '("Find singleton" . crossword-find-singleton)))

(defvar crossword-mode-map nil
  "Keymap for Crossword mode.")

(if crossword-mode-map
    nil
  (setq crossword-mode-map (make-keymap))
  (define-key crossword-mode-map " " 'crossword-erase-command)
  (define-key crossword-mode-map "#" 'crossword-block-command)
  (define-key crossword-mode-map "\C-ct" 'crossword-top-of-column)
  (define-key crossword-mode-map "\C-cb" 'crossword-bottom-of-column)
  (define-key crossword-mode-map "\C-c\C-c" 'crossword-jump-to-cousin)
  (define-key crossword-mode-map "\C-ch" 'crossword-hwords)
  (define-key crossword-mode-map "\C-cv" 'crossword-vwords)
  (define-key crossword-mode-map [down-mouse-1] 'crossword-mouse-set-point)
  (define-key crossword-mode-map [down-mouse-2] 'crossword-mouse-set-point)
  (define-key crossword-mode-map [down-mouse-3] 'crossword-mouse-set-point)
  (define-key crossword-mode-map [mouse-1] 'crossword-mouse-set-point)
  (define-key crossword-mode-map [mouse-2] 'crossword-mouse-block)
  (define-key crossword-mode-map [mouse-3] 'crossword-mouse-erase)
  (define-key crossword-mode-map "\C-c1" 'crossword-find-singleton)
  (define-key crossword-mode-map [menu-bar crossword]
    (cons "Crossword" crossword-menu-map))
  (let ((equivs
	 '((forward-char . crossword-cursor-right)
	   (backward-char . crossword-cursor-left)
	   (previous-line . crossword-cursor-up)
	   (next-line . crossword-cursor-down)
	   (beginning-of-line . crossword-beginning-of-row)
	   (end-of-line . crossword-end-of-row)
	   (beginning-of-buffer . crossword-beginning-of-grid)
	   (end-of-buffer . crossword-end-of-grid))))
    (while equivs
      (substitute-key-definition (car (car equivs))
				 (cdr (car equivs))
				 crossword-mode-map
				 (current-global-map))
      (setq equivs (cdr equivs))))
  (let ((letters
	 '(?A ?B ?C ?D ?E ?F ?G ?H ?I ?J ?K ?L ?M
	      ?N ?O ?P ?Q ?R ?S ?T ?U ?V ?W ?X ?Y ?Z
	      ?a ?b ?c ?d ?e ?f ?g ?h ?i ?j ?k ?l ?m
	      ?n ?o ?p ?q ?r ?s ?t ?u ?v ?w ?x ?y ?z)))
    (while letters
      (define-key crossword-mode-map
	(char-to-string (car letters))
	'crossword-self-insert)
      (setq letters (cdr letters)))))

(defvar crossword-mouse-location nil
  "Location of last mouse-down event, as crossword coords.")

(defun crossword-mouse-set-point (event)
  "Set point with the mouse."
  (interactive "@e")
  (mouse-set-point event)
  (let ((coords (crossword-cursor-coords)))
    (setq crossword-mouse-location coords)
    (crossword-place-cursor (car coords)
                            (cdr coords))))

(defun crossword-mouse-block (event)
  "Place a block with the mouse."
  (interactive "@e")
  (mouse-set-point event)
  (let ((coords (crossword-cursor-coords)))
    (if (equal coords crossword-mouse-location)
        (crossword-block-command))))

(defun crossword-mouse-erase (event)
  "Erase a cell with the mouse."
  (interactive "@e")
  (mouse-set-point event)
  (let ((coords (crossword-cursor-coords)))
    (if (equal coords crossword-mouse-location)
        (crossword-erase-command))))

(defun crossword-find-singleton ()
  "Jump to a one-letter word, if one exists."
  (interactive)
  (let ((row 0)
        (size (crossword-size crossword-grid))
        (result nil))
    (while (and (< row size)
                (null result))
      (let ((column 0))
        (while (and (< column size)
                    (null result))
          (if (crossword-one-letter-p crossword-grid
                                      row column)
              (setq result (cons row column))
            (setq column (+ column 1)))))
      (setq row (+ row 1)))
    (if result
        (crossword-place-cursor (car result)
                                (cdr result))
      (message "No one-letter words."))))

(defvar crossword-changes-authorized nil
  "Are changes currently authorized?")
(make-variable-buffer-local 'crossword-changes-authorized)

(defmacro crossword-authorize (&rest subexprs)
  "Execute subexpressions, authorizing changes."
  `(let ((crossword-changes-authorized t))
     ,@subexprs))

(defun crossword-words-select ()
  (interactive)
  (beginning-of-line)
  (let* ((wordstart (point))
         (word (progn (end-of-line)
                      (buffer-substring wordstart
                                        (point))))
	 (corematch
	  (let ((bestmatch nil)
		(index 0))
	    (while (and index (<= index
				  crossword-words-prefix-len))
	      (let ((match (string-match crossword-words-core
					 word
					 index)))
		(if (and match
			 (<= match crossword-words-prefix-len))
		    (setq bestmatch match
			  index (+ match 1))
		  (setq index nil))))
	    bestmatch))
         (vertical-p crossword-words-vertical-p)
         (window (selected-window)))
    (if (not corematch)
        (error "This word does not fit"))
    (let ((row (if vertical-p
                   (+ crossword-words-row
                      (- crossword-words-prefix-len corematch))
                 crossword-words-row))
          (column (if vertical-p
                      crossword-words-column
                    (+ crossword-words-column
                       (- crossword-words-prefix-len corematch))))
          (i 0))
      (switch-to-buffer crossword-words-crossword-buffer)
      (while (< i (length word))
        (crossword-store-letter crossword-grid
                                row
                                column
                                (aref word i))
        (crossword-update-display crossword-grid
                                  row
                                  column)
        (setq i (+ i 1))
        (if vertical-p
            (setq row (+ row 1))
          (setq column (+ column 1)))))
    (delete-window window)))

(defun crossword-update-display (crossword &optional row column)
  "Called after a change, keeps the display up to date."
  (crossword-authorize
    (if (or (null row)
            (null column))
        (let ((coords (crossword-cursor-coords)))
          (setq row (car coords)
                column (cdr coords))))
    (let ((cousin-coords (crossword-cousin-position crossword
                                                    row
                                                    column)))
      (save-excursion
        (crossword-place-cursor row
                                column)
        (delete-char 2)
        (crossword-insert-cell (crossword-ref crossword
                                              row
                                              column))
        (crossword-place-cursor (car cousin-coords)
                                (cdr cousin-coords))
        (delete-char 2)
        (crossword-insert-cell (crossword-ref crossword
                                              (car cousin-coords)
                                              (cdr cousin-coords)))))))

(defun crossword-hwords ()
  "Pop up a buffer listing horizontal words for current cell."
  (interactive)
  (let ((coords (crossword-cursor-coords)))
    (if (eq (crossword-ref crossword-grid
                           (car coords)
                           (cdr coords))
            'block)
        (error "Cannot use this command on a block"))
    (let ((start (- (cdr coords) 1))
          (end (+ (cdr coords) 1)))
      (while (not (crossword-block-p crossword-grid
                                     (car coords)
                                     start))
        (setq start (- start 1)))
      (while (not (crossword-block-p crossword-grid
                                     (car coords)
                                     end))
        (setq end (+ end 1)))
      (let ((corestart (+ start 1))
            (coreend (- end 1)))
        (while (null (crossword-ref crossword-grid
                                    (car coords)
                                    corestart))
          (setq corestart (+ corestart 1)))
        (if (= corestart end)
            (error "No core for regexp"))
        (while (null (crossword-ref crossword-grid
                                    (car coords)
                                    coreend))
          (setq coreend (- coreend 1)))
        (let ((core "")
              (column corestart)
              (regexp "^"))
          (while (<= column coreend)
            (let ((cell (crossword-ref crossword-grid
                                       (car coords)
                                       column)))
              (if (numberp cell)
                  (setq core (concat core
                                     (char-to-string cell)))
                (setq core (concat core "."))))
            (setq column (+ column 1)))
          (setq column (+ start 1))
          (while (< column corestart)
            (setq regexp (concat regexp ".?"))
            (setq column (+ column 1)))
          (setq regexp (concat regexp core))
          (setq column (+ coreend 1))
          (while (< column end)
            (setq regexp (concat regexp ".?"))
            (setq column (+ column 1)))
          (setq regexp (concat regexp "$"))
          (let ((buffer (get-buffer-create "*Crossword words*"))
                (crossword-buffer (current-buffer)))
            (set-buffer buffer)
            (crossword-words-mode)
            (setq crossword-words-crossword-buffer
                  crossword-buffer)
            (setq crossword-words-core core)
            (setq crossword-words-prefix-len (- corestart
                                                (+ start 1)))
            (setq crossword-words-row (car coords))
            (setq crossword-words-column (+ start 1))
            (setq crossword-words-vertical-p nil)
            (erase-buffer)
            (let ((process
                   (let ((process-connection-type nil))
                     (start-process "egrep"
                                    buffer
                                    crossword-egrep-program
                                    "-i" regexp
                                    crossword-words-file))))
              (set-process-filter process
                                  'crossword--egrep-filter)
              (set-process-sentinel process
                                    'crossword--egrep-sentinel))))))))

(defun crossword-vwords ()
  "Pop up a buffer listing horizontal words for current cell."
  (interactive)
  (let ((coords (crossword-cursor-coords)))
    (if (eq (crossword-ref crossword-grid
                           (car coords)
                           (cdr coords))
            'block)
        (error "Cannot use this command on a block"))
    (let ((start (- (car coords) 1))
          (end (+ (car coords) 1)))
      (while (not (crossword-block-p crossword-grid
                                     start
				     (cdr coords)))
        (setq start (- start 1)))
      (while (not (crossword-block-p crossword-grid
                                     end
                                     (cdr coords)))
        (setq end (+ end 1)))
      (let ((corestart (+ start 1))
            (coreend (- end 1)))
        (while (null (crossword-ref crossword-grid
                                    corestart
                                    (cdr coords)))
          (setq corestart (+ corestart 1)))
        (if (= corestart end)
            (error "No core for regexp"))
        (while (null (crossword-ref crossword-grid
                                    coreend
                                    (cdr coords)))
          (setq coreend (- coreend 1)))
        (let ((core "")
              (row corestart)
              (regexp "^"))
          (while (<= row coreend)
            (let ((cell (crossword-ref crossword-grid
                                       row
                                       (cdr coords))))
              (if (numberp cell)
                  (setq core (concat core
                                     (char-to-string cell)))
                (setq core (concat core "."))))
            (setq row (+ row 1)))
          (setq row (+ start 1))
          (while (< row corestart)
            (setq regexp (concat regexp ".?"))
            (setq row (+ row 1)))
          (setq regexp (concat regexp core))
          (setq row (+ coreend 1))
          (while (< row end)
            (setq regexp (concat regexp ".?"))
            (setq row (+ row 1)))
          (setq regexp (concat regexp "$"))
          (let ((buffer (get-buffer-create "*Crossword words*"))
                (crossword-buffer (current-buffer)))
            (set-buffer buffer)
            (crossword-words-mode)
            (setq crossword-words-crossword-buffer
                  crossword-buffer)
            (setq crossword-words-core core)
            (setq crossword-words-prefix-len (- corestart
                                                (+ start 1)))
            (setq crossword-words-column (cdr coords))
            (setq crossword-words-row (+ start 1))
            (setq crossword-words-vertical-p t)
            (erase-buffer)
            (let ((process
                   (let ((process-connection-type nil))
                     (start-process "egrep"
                                    buffer
                                    crossword-egrep-program
                                    "-i" regexp
                                    crossword-words-file))))
              (set-process-filter process
                                  'crossword--egrep-filter)
              (set-process-sentinel process
                                    'crossword--egrep-sentinel))))))))

(defun crossword-insert-grid (crossword)
  "Insert CROSSWORD into the current buffer."
  (crossword-authorize
    (mapcar 'crossword-insert-row crossword)))

(defvar crossword-unauthorized-change nil
  "Did an unauthorized change occur?")
(make-variable-buffer-local 'crossword-unauthorized-change)

(defun crossword-after-change-function (start end len)
  "Recover if this change is not authorized."
  (if crossword-changes-authorized
      nil
    (setq crossword-unauthorized-change t)))

(defun crossword-post-command-function ()
  "After each command, recover from unauthorized changes."
  (if crossword-unauthorized-change
      (condition-case nil
          (let ((coords (crossword-cursor-coords)))
            (condition-case nil
                (setq crossword-grid (crossword-parse-buffer))
              (error (erase-buffer)
                     (crossword-insert-grid crossword-grid)))
            (crossword-place-cursor (car coords)
                                    (cdr coords)))
        (error (erase-buffer)
               (crossword-insert-grid crossword-grid)
               (crossword-place-cursor 0 0))))
    (setq crossword-unauthorized-change nil))

(defun crossword-parse-buffer ()
  "Parse the crossword grid in the current buffer."
  (save-excursion
    (goto-char (point-min))
    (let* ((line (crossword-parse-line))
           (size (length line))
           (result (make-crossword size))
           (row 1))
      (crossword--handle-parsed-line line 0 result)
      (while (< row size)
        (forward-line 1)
        (setq line (crossword-parse-line))
        (if (not (= (length line) size))
            (error "Rows vary in length"))
        (crossword--handle-parsed-line line row result)
        (setq row (+ row 1)))
      result)))

(defun crossword--handle-parsed-line (line row grid)
  "Take LINE and put it in ROW of GRID."
  (let ((column 0))
    (while line
      (cond ((eq (car line) 'block)
             (crossword-store-block grid row column))
            ((eq (car line) nil)
             (crossword-clear-cell grid row column))
            ((numberp (car line))
             (crossword-store-letter grid row column (car line))))
      (setq line (cdr line))
      (setq column (+ column 1)))))

(defun crossword-parse-line ()
  "Parse a line of a Crossword buffer."
  (beginning-of-line)
  (let ((result nil))
    (while (not (eolp))
      (cond ((eq (char-after (point)) ?#)
             (setq result (cons 'block result)))
            ((eq (char-after (point)) ?.)
             (setq result (cons nil result)))
            ((eq (char-after (point)) ??)
             (setq result (cons nil result)))
            ((looking-at "[A-Za-z]")
             (setq result (cons (char-after (point))
                                result)))
            (t (error "Unrecognized character")))
      (forward-char 1)
      (if (eq (char-after (point)) ?\ )
          (forward-char 1)
        (error "Non-blank between columns")))
    (reverse result)))

(put 'crossword-mode 'mode-class 'special)

(defun crossword--egrep-sentinel (process string)
  "When PROCESS exits, display its buffer."
  (if (eq (process-status process)
          'exit)
      (display-buffer (process-buffer process))))

(defun crossword--egrep-filter (process string)
  "Handle output from PROCESS."
  (let ((buffer (process-buffer process)))
    (save-excursion
      (set-buffer buffer)
      (goto-char (point-max))
      (insert string))
    (display-buffer buffer)))

(defvar crossword-words-crossword-buffer nil
  "The associated crossword buffer.")
(defvar crossword-words-core nil
  "The core of the regexp.")
(defvar crossword-words-prefix-len nil
  "Length of the regexp prefix.")
(defvar crossword-words-row nil
  "Row number where the word can start.")
(defvar crossword-words-column nil
  "Column number where the word can start.")
(defvar crossword-words-vertical-p nil
  "Whether the current search is vertical.")

(defvar crossword-words-mode-map nil
  "Keymap for crossword-words mode.")

(if crossword-words-mode-map
    nil
  (setq crossword-words-mode-map (make-sparse-keymap))
  (define-key crossword-words-mode-map "\r" 'crossword-words-select))

(defun crossword-words-mode ()
  "Major mode for Crossword word-list buffer."
  (interactive)
  (kill-all-local-variables)
  (setq major-mode 'crossword-words-mode)
  (setq mode-name "Crossword-words")
  (use-local-map crossword-words-mode-map)
  (make-local-variable 'crossword-words-crossword-buffer)
  (make-local-variable 'crossword-words-core)
  (make-local-variable 'crossword-words-prefix-len)
  (make-local-variable 'crossword-words-row)
  (make-local-variable 'crossword-words-column)
  (make-local-variable 'crossword-words-vertical-p)
  (run-hooks 'crossword-words-mode-hook))

(provide 'crossword)
